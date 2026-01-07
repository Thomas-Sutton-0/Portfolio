import markdown2
from django.shortcuts import render
from django.shortcuts import redirect
from random import randrange
from markdown2 import Markdown

from . import util

def index(request):
    return render(request, "encyclopedia/index.html", {
        "entries": util.list_entries()
    })

def error(request):
    return render(request, "encyclopedia/error.html")

def page(request, entry_name):
    if entry_name not in util.list_entries():
        return render(request, "encyclopedia/error.html")

    else:
        markdowner = Markdown()
        return render(request, "encyclopedia/entry.html", {
            "entry_name": entry_name,
            "body": markdowner.convert(util.get_entry(entry_name))
        })

def search(request):
    if request.method == "POST":
        entry_name = request.POST['searched']

        if entry_name not in util.list_entries():
            substringlist = []
            checklist = util.list_entries()
            for entry in checklist:
                if entry_name in entry:
                    substringlist.append(entry)

            return render(request, "encyclopedia/search.html", {
                "entries": substringlist,
                "entry_name": entry_name
            })


        else:
            markdowner = Markdown()
            return render(request, "encyclopedia/entry.html", {
                "entry_name": entry_name,
                "body": markdowner.convert(util.get_entry(entry_name))
            })

def newpage(request):
    if request.method == "POST":
        if request.POST['title'] not in util.list_entries():
            f = open("entries/" + request.POST['title'] + ".md", "x")
            f.write(request.POST['markdown'])
            f.close()
            return redirect("wiki/" + request.POST['title'])
        else:
            return newpageerror(request)

    return render(request, "encyclopedia/new_page.html")

def newpageerror(request):
    return render(request, "encyclopedia/npageerror.html")

def edit(request, entry_name):
    if request.method == "POST":
        markdown = request.POST['markdown']
        f = open("entries/" + entry_name + ".md", "w")
        f.write(markdown)
        f.close()
        return redirect("wiki/" + entry_name)
    else:
        f = open("entries/" + entry_name + ".md", "r")
        markdown = f.read()
        f.close()

    return render(request, "encyclopedia/edit.html", {
        "entry_name": entry_name,
        "markdown": markdown
    })

def randompage(request):
    pagelist = util.list_entries()
    num = randrange(0, len(pagelist))
    return redirect("wiki/" + pagelist[num])