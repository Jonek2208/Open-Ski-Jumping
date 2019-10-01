from django.contrib.postgres.fields import JSONField
from django.db import models

class TimestampModel(models.Model):
    created_at = models.DateTimeField(auto_now_add=True)
    updated_at = models.DateTimeField(auto_now=True)

    class Meta:
        abstract = True

class SkiJump(TimestampModel):
    config = JSONField()
