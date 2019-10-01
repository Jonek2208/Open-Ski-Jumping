from rest_framework import permissions
from rest_framework.permissions import SAFE_METHODS
from rest_framework.viewsets import ModelViewSet

from skijump.models import SkiJump
from skijump.serializers import SkiJumpModelSerializer


class IsAdminOrReadOnly(permissions.BasePermission):

    def has_permission(self, request, view):
        return bool(
            request.method in SAFE_METHODS or
            request.user and
            request.user.is_staff
        )

class SkiJumpViewSet(ModelViewSet):
    permission_classes = [IsAdminOrReadOnly,]
    serializer_class = SkiJumpModelSerializer
    queryset = SkiJump.objects.all()
