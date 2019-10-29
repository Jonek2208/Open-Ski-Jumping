from rest_framework.viewsets import ModelViewSet

from accounts.permissions import IsAdminOrReadOnly
from skijump.models import SkiJump
from skijump.serializers import SkiJumpModelSerializer


class SkiJumpViewSet(ModelViewSet):
    permission_classes = [IsAdminOrReadOnly, ]
    serializer_class = SkiJumpModelSerializer
    queryset = SkiJump.objects.all()
