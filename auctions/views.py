from contextlib import nullcontext

from django.contrib.auth import authenticate, login, logout
from django.contrib.auth.decorators import login_required
from django.db import IntegrityError
from django.http import HttpResponse, HttpResponseRedirect
from django.shortcuts import redirect
from django.shortcuts import render
from django.urls import reverse
from . import models

from .models import User, AucListing, Bid, Comment, Category

@login_required(login_url='login')
def index(request):
    try:
        if AucListing.objects.all().count() != 0:
            return render(request, "auctions/index.html", {
                "listings": AucListing.objects.all(),
            })
        else:
            return render(request, "auctions/index.html")
    except:
        return render(request, "auctions/index.html")


def login_view(request):
    if request.method == "POST":

        # Attempt to sign user in
        username = request.POST["username"]
        password = request.POST["password"]
        user = authenticate(request, username=username, password=password)

        # Check if authentication successful
        if user is not None:
            login(request, user)
            return HttpResponseRedirect(reverse("index"))
        else:
            return render(request, "auctions/login.html", {
                "message": "Invalid username and/or password."
            })
    else:
        return render(request, "auctions/login.html")


def logout_view(request):
    logout(request)
    return HttpResponseRedirect(reverse("login"))


def register(request):
    if request.method == "POST":
        username = request.POST["username"]
        email = request.POST["email"]

        # Ensure password matches confirmation
        password = request.POST["password"]
        confirmation = request.POST["confirmation"]
        if password != confirmation:
            return render(request, "auctions/register.html", {
                "message": "Passwords must match."
            })

        # Attempt to create new user
        try:
            user = User.objects.create_user(username, email, password)
            user.save()
        except IntegrityError:
            return render(request, "auctions/register.html", {
                "message": "Username already taken."
            })
        login(request, user)
        return HttpResponseRedirect(reverse("index"))
    else:
        return render(request, "auctions/register.html")

@login_required(login_url='login')
def newlisting(request):
    if request.method == "POST":

        title = request.POST["title"]
        description = request.POST["description"]
        image = request.POST["image"]
        startbid = request.POST["startbid"]
        new = AucListing.objects.create(title=title, description=description, image=image, startbid=startbid, user=request.user)

        price = str(new.startbid)
        if price == str(round(float(new.startbid), 1)):
            price = price + "0"
        if not ('.' in price):
            price = price + ".00"
        new.price = price

        if request.POST["category"] != "":
            newcategory, created = Category.objects.get_or_create(title=request.POST["category"])
            newcategory.listings.add(new)

        new.save()

        return render(request, "auctions/new_listing.html")

    else:
        return render(request, "auctions/new_listing.html")

@login_required(login_url='login')
def listing(request, listingid):
    try:
        auclisting = AucListing.objects.get(id=listingid)
    except:
        return render(request, "auctions/no_listing_error.html")

    if request.method == "POST":
        if request.POST["action"] == "Place Bid":
            if auclisting.bid.count() != 0:
                if float(request.POST["newbid"]) > auclisting.bid.order_by('-amount').first().amount:
                    newbid = Bid.objects.create(amount=request.POST["newbid"], user=request.user)
                    auclisting.bid.add(newbid)

                    price = str(newbid.amount)
                    if price == str(round(float(newbid.amount), 1)):
                        price = price + "0"
                    if not ('.' in price):
                        price = price + ".00"
                    auclisting.price = price

                    newbid.save()
                    auclisting.save()
                else:
                    return render(request, "auctions/low_bid_error.html", {
                        "minbid": auclisting.bid.order_by('-amount').first().amount
                    })
            else:
                if float(request.POST["newbid"]) > auclisting.startbid:
                    newbid = Bid.objects.create(amount=request.POST["newbid"], user=request.user)
                    auclisting.bid.add(newbid)

                    price = str(newbid.amount)
                    if price == str(round(float(newbid.amount), 1)):
                        price = price + "0"
                    if not ('.' in price):
                        price = price + ".00"
                    auclisting.price = price

                    newbid.save()
                    auclisting.save()
                else:
                    return render(request, "auctions/low_bid_error.html", {
                        "minbid": auclisting.startbid
                    })

        elif request.POST["action"] == "Post Comment":
            auclisting.comments.add(Comment.objects.create(text=request.POST["comment"], user=request.user))

        elif request.POST["action"] == "Delete":
            auclisting.delete()
            return redirect("index")

        elif request.POST["action"] == "Close Bid":
            auclisting.closed = True
            if auclisting.bid.count() != 0:
                curbid = auclisting.bid.order_by('-amount').first()
                auclisting.winner = curbid.user
            auclisting.save()

        elif request.POST["action"] == "Add To Watchlist":
            request.user.watchlist.add(auclisting)

        elif request.POST["action"] == "Remove From Watchlist":
            request.user.watchlist.remove(auclisting)

    if auclisting.closed:
        return render(request, "auctions/closed_listing.html", {
            "listing": auclisting,
            "winner": auclisting.winner == request.user
        })

    curUserBid = False
    curbid = auclisting.startbid
    if auclisting.bid.count() != 0:
        curbid = auclisting.bid.order_by('-amount').first().amount
        curUserBid = request.user == auclisting.bid.order_by('-amount').first().user

    userOwner = False
    if request.user == auclisting.user:
        userOwner = True

    onWatchlist = False
    for watchlisting in request.user.watchlist.all():
        if watchlisting == auclisting:
            onWatchlist = True

    bidamount = auclisting.price
    bidnum = auclisting.bid.count()
    newcomments = auclisting.comments.all()


    return render(request, "auctions/listing.html", {
        "listing": auclisting,
        "newbid": bidamount,
        "comments": newcomments,
        "bidnum": bidnum,
        "curUserBid": curUserBid,
        "userOwner": userOwner,
        "onWatchlist": onWatchlist
    })

@login_required(login_url='login')
def category(request, categoryID):
    return render(request, "auctions/category.html", {
        "category": Category.objects.get(id=categoryID).title,
        "listings": Category.objects.get(id=categoryID).listings.all()
    })

@login_required(login_url='login')
def categories(request):
    return render(request, "auctions/categories.html", {
        "categories": Category.objects.all()
    })

@login_required(login_url='login')
def watchlist(request):
    return render(request, "auctions/watchlist.html", {
        "listings": request.user.watchlist.all()
    })