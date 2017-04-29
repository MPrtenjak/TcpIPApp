# About
On an interview a got a question: _"How would you create a program to handle 5000 requests!"_. 

_"I don’t have any real experience with systems aka. web servers with 5.000 simultaneous requests and it is something that web server is supposed to do on strong enough computer."_

As I was sceptic about the nature of the question, a was told that they are talking about TCP/IP server written in .NET used for heavy duty…. All they wanted to hear was that in such a case, I would use queue, which is obvious… 

But the truth to be told I haven’t written a lot of TCP/IP servers as I never had any real need to do so.

__Today is a rainy day and I can use it to write some TCP/IP server/client SW in .NET.__

# Disclaimer

This is not production code. It is meant to show how can you write TCP/IP server/client code in C# using some sync/async code, threading, etc.…

# Usage

Program comes in two flavours; as server or as client. 

## Server options

Detailed explanation in written below in text

* __-d, --dispatcher__	Dispatcher [Thread, Pool, Dynamic]
* __-w, --workload__	Workload [Echo, Time]
* __-r, --realWork__	True if server worker is CPU bound (real work), false if not (using thread.Sleep())
* __-i, --minTime__	Minimal workload time in ms
* __-x, --maxTime__	Maximal workload time in ms
* __-n, --numOfConcurentProceses__	Number of workers
* __-l, --logFileName__	Log file name (if none ==> console)
* __-t, --loggerThreshold__	Logger threshold (Error, Minimal, Debug)
* __-p, --port__	TCP/IP port

## Client options

Detailed explanation in written below in text

* __-f, --flow__	Client flow [Sync, Async, JS, NewAsync]
* __-a, --ipAddress__	TCP/IP address
* __-n, --numOfClients__	Number of clients
* __-u, --useThreads__	Use threads even for async clients
* __-l, --logFileName__	Log file name (if none ==> console)
* __-t, --loggerThreshold__	Logger threshold (Error, Minimal, Debug)
* __-p, --port__	TCP/IP port

# Code

## Logger

Basic logger that can be used for logging on console or into a file. Logger also support buffered logging and threshold.

## Server

Server is classic (blocking) server which means that every new request must run in thread.

## Dispatcher

This is the core of the server. Server receives a request and immediately dispatches it to a dispatcher which is used to process the request and create response. There are three different dispatchers:

### Thread dispatcher

This is the easiest and dumbest way to dispatch a request as it just creates a new thread for each request. If there are 5.000 request it will (try to) create 5000 threads. You can easily test that it will fail if there are too many requests.

_In one window create a server (using thread dispatcher) with client workload of 2 second_

```DOS
TcpIPApp.exe server -d thread -i 2000 -x 2000
```

_Open few more windows and in each send 1000 (or so) requests_

```DOS
TcpIPApp.exe client -a 192.168.2.101 -n 1000
```

### Pool dispatcher

This one uses a pool of threads and each thread is responsible for serving clients. So, if we create a pool with 10 threads, 10 clients can be served at the same time. 

This is much better dispatcher strategy as it will not blow off the server creating too many threads. But it fails quickly, because if all the pool threads are serving clients there are nobody that can accept new clients and clients fails because server is not accepting them.

_Create a server (using pool) with 20 workers and client workload between 1 and 4 seconds_

```DOS
TcpIPApp.exe server -d pool -n 20 -i 1000 -x 4000
```

### Dynamic dispatcher

This one is a step further than pool dispatcher. It uses one thread to accept each request, so in theory it is possible to accept few 1000 clients. 

Every new client is put in a queue and on the other end of the queue there are workers (as in pool dispatcher) that accept request from queue and process them. 

This kind of the server will accept every client, but if there are too many clients they must wait too much time in a queue so timeouts will accrue. But strong enough server or cluster of servers will be able to serve clients fast enough.

__So, this is the kind of server (dispatcher) that is the solution for the question in the beginning of the story.__

## Workload

Every server needs to produce result, so there are two kind of responses. Echo just echo back anything that it receives from the client, Time response with current date and time.

To simulate some real work, there is the option to set the minimal and maximal time used to create response. You can even simulate real work (CPU is doing something for real) or just waiting (Thread.Sleep()) for some I/O or database data.

If you use real work: 

```DOS
TcpIPApp.exe server -d pool -n 20 -i 1000 -x 4000 -r
```

You will easily produce 100% CPU usage with just few workers and clients.

## Clients

To test the server, we need clients so there are four__different kind of clients__

### SyncClient

This is the easiest kind of client, but it blocks the program when talking with server. So, to use this one you must use a working thread.

### ClasicAsyncClient

This one is asynchronous, so it will not block the main program. This kind of clients are difficult to use and the version presented here is also blocking because it waits for response.

### JSLikeAsyncClient

Last year I became quite a fan of JavaScript and famous call-backs. This client is written with same structures as previous (classic async client) but uses callbacks (as JS) to present the result. This one is easier to use and it works asynchronous.

### NewAsyncClient

This one uses Tasks so it is asynchronous and easy to work with

## Client Runner
This is the code used to create N clients and it uses N threads or N asynchronous clients (if it is possible).