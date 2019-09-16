from .base import *

DATABASES = {
    'default': {
        'ENGINE': 'django.db.backends.postgresql',
        'NAME': os.environ.get('POSTGRES_DB', 'skijump'),
        'USER': os.environ.get('POSTGRES_USER', 'skijump'),
        'PASSWORD': os.environ.get('POSTGRES_PASSWORD', 'skijump'),
        'HOST': os.environ.get('POSTGRES_HOST', 'localhost'),
    }
}
