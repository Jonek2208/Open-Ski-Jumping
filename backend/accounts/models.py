from django.contrib.auth.models import AbstractUser
from django.db import models

from skijump.models import TimestampModel, SkiJump


class CustomUser(AbstractUser):
    pass


class Score(TimestampModel):
    user = models.ForeignKey(CustomUser, on_delete=models.CASCADE)
    skijump = models.ForeignKey(SkiJump, on_delete=models.CASCADE)
    score = models.IntegerField(default=0)
