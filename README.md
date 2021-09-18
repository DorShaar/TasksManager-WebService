# TasksManager-WebService

## About that project
TasksManager-WebService is ASP.Net application that waits for CRUD (Create, Read, Update, Delete) http/s operation and help you manage your tasks seperated by groups.

That service was build as part of [TasksManager Project](https://github.com/DorShaar/TasksManager "TasksManager")
and from TasksManager Version 1.1 I decided to seperate it into different projects. This project is the server side which is responsible for handling requests from the client and update the database accordingly.


## What Tasks and Groups is consisted of
Each task has status of "Open", "Closed" or "On-Work".\
Each task can have:

* Note.
* Task Triangle - Gives the ability to control your tasks in three dimensions:\
a. Time (What is the dead-line).\
b. Quality (What is the contnet of the task).\
c. Resources (Who work on that task).

Group's status is dependent in all of the tasks statuses it conatins.\

## The GUI Part
There is a web application part for this project, created in react, and gives some GUI to the regular TaskMananger.
Look for [Tasker.UI](https://github.com/DorShaar/TasksManager-WebService/tree/master/Tasker.UI "Tasker.UI")

## Usage
See at [TasksManager Project](https://github.com/DorShaar/TasksManager "TasksManager").

## Development patterns and information I used and learn in that project

IOC (Invertion of control),\
Controllers,\
Custome Middlewares,\
API testing,\
Union architecture pattern,
Cors (Cross-origin resource sharing),\
GoogleDrive Api,\
Hosted Service,\
Dispose pattern,\
Email sender,
