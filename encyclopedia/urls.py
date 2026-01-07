from django.urls import path

from . import views

urlpatterns = [
    path("", views.index, name="index"),
    path("Error", views.error, name="error"),
    path("Random", views.randompage, name="random"),
    path("wiki/Search", views.search, name="search"),
    path("Edit<str:entry_name>",views.edit, name="edit"),
    path("wiki/<str:entry_name>", views.page, name="page"),
    path("NewPage", views.newpage, name="newpage"),
    path("NewPageError", views.newpageerror, name="newpageerror")
]
