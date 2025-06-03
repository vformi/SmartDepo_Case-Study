# Smart Depo – Case Study
A simple ASP.NET Core Web API simulating mission planning for trams queued on a single rail.

### Assumptions
1. All trams are positioned in a single line on one rail (positions 0 to N).
2. Trams at positions 0 to C are assumed to already have a mission.
3. Trams at positions (C+1) to N are waiting for a mission.
4. The first available tram (lowest index without a mission) is selected for planning.

### How to Run

1. Clone the repository
```bash
git clone https://github.com/vformi/
```

```bash
cd SmartDepo_CaseStudy
```

2. Run the application
```bash
dotnet run
```

3. Open Swagger UI

https://localhost:5000/swagger

(Port may vary; check the console output of dotnet run)

### Concurrency Simulation

A semaphore lock ensures that only one client can plan at a time.

The lock is held artificially for 5 seconds after assignment to simulate real-world planning time.

### Design Notes
Uses PriorityQueue<Tram, int> to track available trams by index. Inspired by a priority queue [example](https://www.geeksforgeeks.org/c-priorityqueue/). The tram index is used as the priority to efficiently identify the first available tram.

### Testing

Use Swagger UI or Postman to test all endpoints. There is available postman collection *(Skoda_Digital_Depo.postman_collection.json)* in the repository. 

Try multiple quick requests to /api/v1/trams/assign-mission to simulate conflict.