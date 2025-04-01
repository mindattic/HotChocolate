# HotChocolate

🧠 GraphQL Gateway Example with HotChocolate
This project demonstrates a self-hosted GraphQL server built using HotChocolate in C#. It acts as a gateway that unifies data from:

🛰️ A public REST API (JSONPlaceholder)

🗂️ A local SQLite database (payments.db)

🚀 Features
GraphQL API with a single unified endpoint

Fetches user profiles from a remote API:
https://jsonplaceholder.typicode.com/users/{id}

Fetches payment history from a local SQLite database

Combines both sources into one User GraphQL type

Built with:

ASP.NET Core

HotChocolate v13

Entity Framework Core + SQLite

🧪 Try It
Run the project:

bash
Copy
Edit
dotnet run
Open your browser:
http://localhost:5095/graphql

Paste this GraphQL query and click Run:

graphql
Copy
Edit
query {
  user(id: 1) {
    id
    name
    email
    payments {
      amount
      date
    }
  }
}
🛠 How It Works
The user(id) query:

Calls the JSONPlaceholder API to get the user's name and email

Queries payments.db for matching payment records

Merges both into one response

📦 Technologies Used
.NET 7

HotChocolate (GraphQL)

EF Core (SQLite)

Example Query:

query {
  user(id: 1) {
    id
    name
    email
    payments {
      amount
      date
    }
  }
}
