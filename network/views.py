from django.contrib.auth import authenticate, login, logout
from django.db import IntegrityError
from django.http import HttpResponse, HttpResponseRedirect, JsonResponse
from django.shortcuts import render, redirect
from django.urls import reverse

from .models import User, Post


def index(request):
    return render(request, "network/index.html")


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
            return render(request, "network/login.html", {
                "message": "Invalid username and/or password."
            })
    else:
        return render(request, "network/login.html")


def logout_view(request):
    logout(request)
    return HttpResponseRedirect(reverse("index"))


def register(request):
    if request.method == "POST":
        username = request.POST["username"]
        email = request.POST["email"]

        # Ensure password matches confirmation
        password = request.POST["password"]
        confirmation = request.POST["confirmation"]
        if password != confirmation:
            return render(request, "network/register.html", {
                "message": "Passwords must match."
            })

        # Attempt to create new user
        try:
            user = User.objects.create_user(username, email, password)
            user.save()
        except IntegrityError:
            return render(request, "network/register.html", {
                "message": "Username already taken."
            })
        login(request, user)
        return HttpResponseRedirect(reverse("index"))
    else:
        return render(request, "network/register.html")

def new_post(request):
    if request.method == "POST":
        text = request.POST["text"]
        new = Post.objects.create(text=text, poster=request.user, likes=0)
        new.save()


    return render(request, "network/new_post.html")

def posts(request):
    start = int(request.GET.get("start") or 0)
    end = int(request.GET.get("end") or (start + 10))
    too_large = False

    text = []
    date = []
    likes = []
    poster = []
    ids = []
    posts = Post.objects.all()
    for post in posts:
        poster.append(post.poster.username)
        likes.append(post.likes)
        text.append(post.text)
        date.append(post.date)
        ids.append(post.id)

    text.reverse()
    date.reverse()
    poster.reverse()
    likes.reverse()
    ids.reverse()

    if (end - start) < (len(text) - start):
        text = text[start:end]
        date = date[start:end]
        poster = poster[start:end]
        likes = likes[start:end]
        ids = ids[start:end]
        too_large = True
    else:
        end = len(text)
        text = text[start:end]
        date = date[start:end]
        poster = poster[start:end]
        likes = likes[start:end]
        ids = ids[start:end]

    return JsonResponse({
        "texts": text,
        "dates": date,
        "posters": poster,
        "likes": likes,
        "ids": ids,
        "too_large": too_large,
        "user": request.user.username
    })

def userposts(request):
    start = int(request.GET.get("start") or 0)
    end = int(request.GET.get("end") or (start + 9))
    user = str(request.GET.get("user"))
    too_large = False

    text = []
    date = []
    likes = []
    poster = []
    ids = []
    posts = Post.objects.all()
    for post in posts:
        if post.poster.username == user:
            poster.append(post.poster.username)
            likes.append(post.likes)
            text.append(post.text)
            date.append(post.date)
            ids.append(post.id)

    text.reverse()
    date.reverse()
    poster.reverse()
    likes.reverse()
    ids.reverse()

    if end < len(text):
        text = text[start:end]
        date = date[start:end]
        poster = poster[start:end]
        likes = likes[start:end]
        ids = ids[start:end]
        too_large = True
    else:
        end = len(text)
        text = text[start:end]
        date = date[start:end]
        poster = poster[start:end]
        likes = likes[start:end]
        ids = ids[start:end]

    return JsonResponse({
        "texts": text,
        "dates": date,
        "posters": poster,
        "likes": likes,
        "too_large": too_large,
        "ids": ids
    })

def profile(request, username):
    if User.objects.filter(username=username).count() == 0:
        return render(request, "network/profile_error.html")

    if not request.user.is_authenticated:
        return render(request, "network/profile.html", {
            "username": username,
            "following": User.objects.filter(username=username).first().following.all().count(),
            "followers": User.objects.filter(username=username).first().followers.all().count(),
            "currentuser": True
        })

    elif request.user.username == username:
        return render(request, "network/profile.html", {
            "username": username,
            "following": User.objects.filter(username=username).first().following.all().count(),
            "followers": User.objects.filter(username=username).first().followers.all().count(),
            "currentuser": True
        })

    elif request.user.following.filter(username=username).exists():
        return render(request, "network/profile.html", {
            "username": username,
            "following": User.objects.filter(username=username).first().following.all().count(),
            "followers": User.objects.filter(username=username).first().followers.all().count(),
            "currentuser": False,
            "follow": True
        })

    else:
        return render(request, "network/profile.html", {
            "username": username,
            "following": User.objects.filter(username=username).first().following.all().count(),
            "followers": User.objects.filter(username=username).first().followers.all().count(),
            "currentuser": False,
            "follow": False
        })

def follow(request):
    username = str(request.GET.get("user"))
    curUser = request.user
    folUser = User.objects.filter(username=username).first()

    if curUser.following.filter(username=username).exists():
        curUser.following.remove(folUser)

    else:
        curUser.following.add(folUser)

    return JsonResponse({
        "response": "Complete",
    })

def like(request):
    postId = str(request.GET.get("postId"))
    post = Post.objects.get(id=postId)

    if request.user.is_authenticated:
        if request.user.liked.filter(id=post.id).exists():
            request.user.liked.remove(post)
            post.likes = post.likes - 1
        else:
            request.user.liked.add(post)
            post.likes = post.likes + 1

        post.save()
        return JsonResponse({
            "likes": post.likes
        })

    else:
        return JsonResponse({
            "likes": post.likes
        })


def following(request):
    if not request.user.is_authenticated:
        return render(request, "network/signed_in_error.html")

    return render(request, "network/following.html")

def followingposts(request):
    start = int(request.GET.get("start") or 0)
    end = int(request.GET.get("end") or (start + 9))
    user = request.user
    following = user.following.all()
    too_large = False

    text = []
    date = []
    likes = []
    poster = []
    ids = []
    for follow in following:
        posts = follow.post_set.all()
        for post in posts:
            poster.append(post.poster.username)
            likes.append(post.likes)
            text.append(post.text)
            date.append(post.date)
            ids.append(post.id)

    text.reverse()
    date.reverse()
    poster.reverse()
    likes.reverse()
    ids.reverse()

    if end < len(text):
        text = text[start:end]
        date = date[start:end]
        poster = poster[start:end]
        likes = likes[start:end]
        ids = ids[start:end]
        too_large = True
    else:
        end = len(text)
        text = text[start:end]
        date = date[start:end]
        poster = poster[start:end]
        likes = likes[start:end]
        ids = ids[start:end]

    return JsonResponse({
        "texts": text,
        "dates": date,
        "posters": poster,
        "likes": likes,
        "ids": ids,
        "too_large": too_large,
        "user": request.user.username
    })

def edit(request):
    postId = str(request.GET.get("postId"))
    start = str(request.GET.get("start") or 0)
    post = Post.objects.get(id=postId)
    newText = str(request.GET.get("text"))

    if request.user == post.poster:
        post.text = newText
        post.save()
        return JsonResponse({
            "response": "Complete"
        })

    else:
        return render(request, "network/edit_error.html")