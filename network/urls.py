
from django.urls import path

from . import views

urlpatterns = [
    path("", views.index, name="index"),
    path("login", views.login_view, name="login"),
    path("logout", views.logout_view, name="logout"),
    path("register", views.register, name="register"),
    path("newPost", views.new_post, name="new post"),
    path("posts", views.posts, name="posts"),
    path("userposts", views.userposts, name="userposts"),
    path("follow", views.follow, name="follow"),
    path("following", views.following, name="following"),
    path("followingposts", views.followingposts, name="followingposts"),
    path("like", views.like, name="like"),
    path("edit", views.edit, name="edit"),
    path("<str:username>", views.profile, name="profile"),
]
