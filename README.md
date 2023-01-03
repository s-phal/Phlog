# Phlog
A Photo Management application that I currently use for my photography site.

Project was developed using:
- ASP.NET MVC
- Entity Framework
- Bootstrap 5
- [Masonry Grid Layout Library](https://masonry.desandro.com/)
- PostgreSQL


Hosting Environment:
- Debian 11
- Apache Web Server that proxies to the Kestrel Server
- [My Photography Site](http://www.samphal.com)
- You can demo the app with the provided credentials [here](http://phlogdemo.samphal.com/admin)

# Screenshots
Dashboard
![dashboard](https://user-images.githubusercontent.com/111925825/210405248-07ced1a3-ed9d-4abe-9106-d82707ec13c9.jpg)

Front Page
![front](https://user-images.githubusercontent.com/111925825/210410676-97476625-4c4d-4422-bc19-5504f04aa112.jpg)

# Installation
This project was developed and customized specifically for my needs but others may find it useful.
- Have PostgreSQL installed
- Change the connection string in the appsettings.json file
- Apply Migration during first page load
- Done

# Known Issues
Images on the grid layout may appear overlapped. This is a [bug with the Mason library](https://github.com/desandro/masonry/issues/1147)

