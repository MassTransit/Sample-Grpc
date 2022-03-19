First, run the server:

`dotnet run --port 19796`

Then, run the worker client:

`dotnet run --no-build --servers http://127.0.0.1:19796/ --port 19797`

An exchange of messages should occur between the worker node and the server, load balanced.
