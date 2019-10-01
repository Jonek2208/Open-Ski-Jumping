from rest_framework import serializers

from skijump.models import SkiJump


class SkiJumpModelSerializer(serializers.ModelSerializer):

    class Meta:
        model = SkiJump
        fields = ('config', 'created_at', 'updated_at')
