"""
WSGI config for backend project.

It exposes the WSGI callable as a module-level variable named ``application``.

For more information on this file, see
https://docs.djangoproject.com/en/2.2/howto/deployment/wsgi/
"""

import os
from pathlib import Path
from dotenv import load_dotenv

from django.core.wsgi import get_wsgi_application

env_path = Path('.env')
load_dotenv(dotenv_path=env_path)

os.environ.setdefault('DJANGO_SETTINGS_MODULE', 'backend.settings')

application = get_wsgi_application()
