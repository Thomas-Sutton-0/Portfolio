from django.contrib.auth.models import AbstractUser
from django.db import models

class User(AbstractUser):
    following = models.ManyToManyField('self', related_name='followers', blank=True, symmetrical=False)
    liked = models.ManyToManyField('Post', related_name='liked', blank=True, symmetrical=False)

class Post(models.Model):
    poster = models.ForeignKey(User, on_delete=models.CASCADE)
    text = models.TextField()
    likes = models.IntegerField(default=0)
    date = models.DateTimeField(auto_now_add=True)