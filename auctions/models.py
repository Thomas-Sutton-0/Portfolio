from django.contrib.auth.models import AbstractUser
from django.db import models

class User(AbstractUser):
    watchlist = models.ManyToManyField('AucListing', blank=True, related_name='watchlist')

class Bid(models.Model):
    amount = models.FloatField()
    user = models.ForeignKey(User, on_delete=models.CASCADE)

    def __str__(self):
        return str(self.amount)

class Comment(models.Model):
    user = models.ForeignKey(User, on_delete=models.CASCADE)
    text = models.TextField()

class AucListing(models.Model):
    title = models.CharField(max_length=100)
    description = models.TextField()
    image = models.CharField(max_length=300, blank=True, null=True)
    bid = models.ManyToManyField(Bid, blank=True, related_name='bids')
    comments = models.ManyToManyField(Comment, blank=True, related_name='comments')
    startbid = models.FloatField()
    price = models.CharField(max_length=30, blank=True, null=True)
    user = models.ForeignKey(User, on_delete=models.CASCADE, blank=True, null=True, related_name='user')
    closed = models.BooleanField(default=False)
    winner = models.ForeignKey(User, on_delete=models.CASCADE, blank=True, null=True, related_name='winner')

    def __str__(self):
        return str(self.title)

class Category(models.Model):
    title = models.CharField(max_length=100)
    listings = models.ManyToManyField(AucListing, blank=True, related_name='categories')

    def __str__(self):
        return str(self.title)