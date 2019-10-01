from django.contrib import admin
from django.contrib.auth.admin import UserAdmin

# from accounts.models import User
from .models import CustomUser


class CustomUserAdmin(UserAdmin):
    pass

admin.site.register(CustomUser, CustomUserAdmin)
