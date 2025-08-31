# StripeBet

A modern .NET 8 web application that combines online betting with secure payment processing through Stripe integration. Users can place color-based bets, manage their account balance, and process deposits/withdrawals seamlessly.

## 🎯 Project Overview

StripeBet is a gambling web application built with ASP.NET Core MVC that allows users to:
- Register and authenticate securely
- Place bets on color-based games (Red vs Black)
- Deposit funds using Stripe payment processing
- Withdraw winnings through Stripe payouts
- Track betting history and transaction records
- Manage account balance in real-time

## 🎮 How It Works

### Betting System
- Users bet on either **Red** or **Black**
- A random number (1-100) is generated
- **Even numbers = Red**, **Odd numbers = Black**
- Winning bets double the user's money
- Losing bets deduct the bet amount from balance

### Payment Integration
- **Deposits**: Secure payment processing via Stripe Checkout
- **Withdrawals**: Automated payouts through Stripe API
- Real-time balance updates after transactions

## ⚡ Key Features

- **🔐 User Authentication**: Secure registration/login with password hashing
- **🎲 Real-time Betting**: Instant bet processing with live results
- **💳 Stripe Integration**: Full payment processing for deposits and withdrawals
- **📊 Transaction History**: Complete audit trail of all financial activities
- **🎯 Session Management**: Secure user sessions with balance tracking
- **📱 Responsive Design**: Bootstrap-powered responsive UI
- **🔒 Data Security**: SQL Server with Entity Framework Core

## 🛠️ Technologies Used

### Backend
- **.NET 8** - Latest framework version
- **ASP.NET Core MVC** - Web framework
- **Entity Framework Core** - ORM with SQL Server
- **Stripe.NET** - Payment processing SDK

### Frontend
- **Razor Views** - Server-side rendering
- **Bootstrap 5** - Responsive CSS framework
- **jQuery** - Client-side functionality

### Database
- **SQL Server** - Primary database
- **Entity Framework Migrations** - Database versioning

### Architecture
- **Repository Pattern** - Service layer abstraction
- **Dependency Injection** - Built-in DI container
- **Session State** - User state management

## 🚀 Setup Instructions

### Prerequisites
- .NET 8 SDK
- SQL Server (LocalDB or full instance)
- Stripe Account (for API keys)

### Installation

1. **Clone the repository**

2. **Configure Database**

Update the connection string in `appsettings.json`:

3. **Add Stripe API Keys**

    "SecretKey": "Put_Your_Secret_Key_Here",
    "PublishableKey": "Put_Your_Publishable_Key_Here"

4. **Run Database Migrations**

5. **Start the Application**

6. **Access the App**
- Navigate to `https://localhost:7212`
- Use demo account

## 💡 Key Business Logic

### Betting Algorithm

### Payout System
- **Winning bet**: User receives their bet amount as winnings
- **Losing bet**: Bet amount is deducted from balance
- All transactions are recorded for audit purposes

## 🔒 Security Features

- **Password Hashing**: ASP.NET Core Identity password hasher
- **Session Management**: Secure session-based authentication
- **Input Validation**: Model validation on all forms
- **SQL Injection Protection**: Entity Framework parameterized queries
- **API Key Security**: Development keys excluded from repository

## 🔧 Configuration

### Required Environment Variables
- `Stripe:SecretKey` - Your Stripe secret key
- `Stripe:PublishableKey` - Your Stripe publishable key
- `ConnectionStrings:DefaultConnection` - SQL Server connection string

### Database Schema
The application uses Entity Framework migrations to manage the database schema:
- **Users** - User accounts and authentication
- **Transactions** - Financial transaction history
- **BetResultViewModel** - Betting history and results

## 📝 API Endpoints

### Authentication
- `GET /Account/Login` - Login page
- `POST /Account/Login` - Process login
- `GET /Account/Register` - Registration page
- `POST /Account/Register` - Process registration
- `POST /Account/Logout` - Logout user

### Betting
- `GET /Bet` - Betting interface
- `POST /Bet` - Place a bet
- `GET /Bet/Result` - View bet results

### Payments
- `POST /Home/BuyAsync` - Process deposit
- `POST /Home/PayoutAsync` - Process withdrawal

## 🐛 Troubleshooting

### Common Issues

1. **Database Connection Errors**
   - Verify SQL Server is running
   - Check connection string format
   - Ensure database exists after migration

2. **Stripe Payment Failures**
   - Verify API keys are correct
   - Check Stripe dashboard for webhook status
   - Ensure test mode keys for development

3. **Migration Issues**
   - Run `dotnet ef database drop` to reset
   - Then `dotnet ef database update` to recreate

## 📄 License

This project is for educational purposes. Please ensure compliance with local gambling regulations before deployment.
