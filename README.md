# powerplant-coding-challenge

This is my implementation of the coding challenge.

The API folder contains the web API including a web socket endpoint. Details about the approach to solve the issue are documented as comments with the algorithm.

The Client folder contains a web socket client which connects to the API and reports on any production plan requests.

If you rollback to commit 2d2f252 you will find an implementation using SignalR for the web socket part. I replaced this because I figured the challenge was actually about web sockets. SignalR makes working easier, but might be difficult to use from some technologies.
