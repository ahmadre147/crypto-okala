# Answers to Technical Questions

## 1. How long did you spend on the coding assignment? What would you add to your solution if you had more time?
I spent approximately 7~8 hours on the coding assignment. If I had more time, I would focus on improving the following areas:
- **Comprehensive Unit Tests**: I would write more detailed and with more corner case unit tests. Because of all the busyness that I had, I couldn't spend so much time in the tests and I was more focused on hard tests for the first step. Although I tried hard to create enough soft tests but I guess It's never that much enough :)
- **Error Handling**: Add more robust error handling, especially for network failures and API response validation.
- **Documentation**: Provide more detailed documentation for each service and method to improve maintainability.

## 2. What was the most useful feature that was added to the latest version of your language of choice?

Since you are talking about the **latest version** I can example **Primary Constructors** and **Collection Expressions**.
You can actively see it in the code that I provided for you, they enhance readability and reduce boilerplate (the things I love).
But to be more straightforward, I can also name **record** as a safe immutable useful feature in the area. Sure it's not in the latest .NET version but
it's surely one the most useful ones. I've used record, record struct and other iterations of this feature when it was needed
and surely in this project whenever an immutable retrieval was included (like fetching from exchanges external services) records were used.

```
public record CoinMarketCapErrorDto
{
    [JsonPropertyName("status")]
    public ErrorStatusDto Status { get; set; }
}
```


## 3. How would you track down a performance issue in production? Have you ever had to do this?
1. **Monitoring and Logging**: First, check monitoring dashboards (like Grafana) and analyze logs to find there is an issue.
2. **Profiling**: Performance profiling tools (like dotTrace) to capture live metrics for bottlenecks.
3. **Load Testing**: Simulate similar traffic in a staging environment to replicate the issue.

Yes, I have had to do it, especially with memory leaks and inefficient database queries, and these steps helped address them efficiently.
We noticed the issue by simply checking a constant restarting container and tracked down and solved this issue by these steps.

## 4. What was the latest technical book you have read or tech conference you have been to? What did you learn?
The latest technical book I read was **Software Engineering: A Modern Approach** by Marco Tulio Valente .
The key takeaway was the importance of designing software in a way that separates business logic from external dependencies.
Also, I enhanced my knowledge in a better way of design patterns (good to mention this book had a website and it was really good for people like me)

## 5. What do you think about this technical assessment?
This assessment does a great job reflecting real-world challenges like integrating third-party services, managing errors, and writing clean, efficient code. It offers a clear look at the candidate's coding style, approach to problem-solving, and architectural thinking.
Also, I had to parse the swagger of coinmarket cap myself due to it's unusual server error, so if that counts, that another learning from this assignment :)

## 6. Please, describe yourself using JSON.
```json
{
  "name": "Ahmadreza Enayati",
  "role": "Software Engineer",
  "experienceYears": 3,
  "languages": ["C#", "Java" ,"Python", "C"],
  "interests": ["Backend Development", "System Design", "Deep Learning"],
  "hobbies": ["Reading", "Watching Movies", "Traveling", "Restaurant & Cafe"]
}
```
