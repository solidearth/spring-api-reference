spring-api-reference
====================

This repository contains reference implementations for the Spring API (https://developer.solidearth.com).

Current API reference implementations

* Python

# Python

To install the dependencies for the python reference implementation, run the following:

```
pip install requests
pip install Flask
```

The python reference implementation uses the Flask micro framework. The application is started with Debug settings to enable auto-reload of file changes and helpful debug message. This should be turned off for production deployments.

```python
app.run(debug=True)
```

To start the debug server, add your API key and Flask secret key to config.py and then run the following:

```
python app.py
```

then go to http://127.0.0.1:5000/
