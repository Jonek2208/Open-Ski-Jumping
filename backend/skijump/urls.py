from django.urls import path
from rest_framework.routers import DefaultRouter

from skijump.views import SkiJumpViewSet

router = DefaultRouter()

router.register('skijumps', SkiJumpViewSet)

urlpatterns = [

]
urlpatterns += router.urls

