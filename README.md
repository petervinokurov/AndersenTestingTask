# Andersen test task

* To run the project click F5.
* After the project startup, you should be redirected to Swagger page.
* To prevent security risks in the request pipeline added API rate limiter with settings 4 requests in 12 seconds window.
* CORS also setup to accept requests.
* Domain logic moved into a separate dll for code reusing in the future.
* Implemented logging to observe execution and middle results of the system.
* To avoid unnecessary requests to data implemented in the memory cache.
* Find tests in .Tests project.