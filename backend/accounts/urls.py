from django.urls import path, include

app_name = 'accounts'

urlpatterns = [
    path(r'', include('djoser.urls')),
    path(r'', include('djoser.urls.authtoken')),
]

