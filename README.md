spring-api-reference
====================

This repository contains reference implementations for the Spring API (https://developer.solidearth.com).

Current API reference implementations

* Python
* Node.js
* JavaScript
* C#

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

# Node.js

To install the dependencies for the Node.js reference implementation first install Node.js and then run the following:

```
npm install
```

To start the express.js server, add your API key and session secret key to config.json and run the following:

```
node app.js
```

# JavaScript

The JavaScript reference implementation uses JQuery and the Yelp API to find bars around listings using client calls (no server necessary).

To start the application, add your Spring API key and Yelp API key to the API object in barfind.html and open the file in a web browser of your choice.

# CSharp

The C# reference implementation uses ASP.NET MVC 4.

To start the application, add your Spring API to the web.config and start the application in Visual Studio using the following:

```
Ctrl + F5
```
