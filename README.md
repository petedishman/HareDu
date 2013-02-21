HareDu
======
HareDu is a .NET client and library that consumes the RabbitMQ REST API and is used to manage and monitor a RabbitMQ server or cluster.


Usage
=====
1.) Setup your HareDu client by calling the ConnectTo method and passing in the URL (http://<IP_address>:<port>) to the RabbitMQ server

		var client = HareDuFactory.New(x => x.ConnectTo("http://localhost:15672"));

The above represents the minimm configuration of the client but HareDu also exposes the EnableLogging method for logging and the TimeoutAfter method for setting the timeout threshold.

2.) Setup a resource factory using your user credentials

		var yourResourceFactory = client.Factory<YourResourcesInterface>(x => x.Credentials(<username>, <password>));

3.) Call your properties and methods that are available to you based on your resource factory you setup in step 2.

        var someFakeResponse = yourResourceFactory.Exchange
                                                  .New(x => x.Exchange(exchange),
                                                       x =>
                                                           {
                                                               x.IsDurable();
                                                               x.UsingRoutingType(y => y.Fanout());
                                                           },
                                                       x => x.VirtualHost(virtualHost))
                                                  .Response();

Be sure to call either Response or Data (depending on whether you are returning data - HTTP GET - or a server response - HTTP POST/PUT/DELETE) if you plan on returning a result. Otherwise the method will return a Task<T>.


Assumptions
===========
1.) You have RabbitMQ running in some environment that is reachable from the machine that you are running HareDu applications on
2.) You know the URL and port to access the RabbitMQ REST API you want to interact with
3.) You have some valid credentials to communite with the RabbitMQ server (default credentials are: username => guest, password => guest)


Dependencies
============
.NET Framework
JSON.NET
ASP.NET WebAPI
Common.Logging


Tested
======
Windows Server 2008 R2
RabbitMQ 2.8.7 and 3.0.1
Erlang OTP R15B02 (x64)
.NET 4.0 Framework

