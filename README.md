# 🏥 Doctor Appointment System (MVC + API)

A full-stack medical appointment management system that allows patients to book appointments online and enables doctors to manage their schedules.  
This project includes:

- **ASP.NET Core Web API** (Backend)
- **ASP.NET Core MVC** (Frontend UI)
- **SQL Server Database**
- **Entity Framework Core**
- **Bootstrap UI Template (Medilab)**

---

## 📌 1. Project Overview

The system helps patients find doctors, view availability, and book appointments easily.  
Doctors can manage their schedules, and admins can monitor the overall system.

### ✔ Features
- Patient Registration & Login  
- Doctor Registration & Profile  
- Appointment Booking  
- Doctor Availability Management  
- Notifications  
- Medical Specialties  
- Admin Dashboard  
- Clean UI using Bootstrap Template

---

## 📌 2. System Architecture


- **MVC Project:** Handles UI, views, routing, static pages.  
- **API Project:** Handles controllers, logic, database operations.  
- **Database:** Designed using normalized ERD.

---

## 📌 3. ERD (Database Diagram)

(Here insert your screenshot or link)

- Users  
- Patients  
- Doctors  
- Specializations  
- DoctorAvailability  
- Appointments  
- Notifications  

Relationships:
- One-to-one: User ↔ Patient, User ↔ Doctor  
- One-to-many: Doctor → Availability  
- One-to-many: Patient → Appointments  
- One-to-many: Doctor → Appointments  
- One-to-many: Appointment → Notifications  

---

## 📌 4. Technologies Used

### Backend (API)
- ASP.NET Core Web API
- Entity Framework Core
- SQL Server
- LINQ
- JWT Authentication (Optional)

### Frontend (MVC)
- ASP.NET Core MVC
- Bootstrap 5
- Medilab Template
- jQuery / JavaScript
- AJAX (Optional)

---

## 📌 5. How to Run the Project

### ⚙ Backend (API)
1. Open solution in Visual Studio  
2. Set **Project.API** as Startup  
3. Update `appsettings.json` with your SQL connection  
4. Run migrations:
5. Run the project → Swagger will open automatically  

### 🎨 Frontend (MVC)
1. Set **Project.MVC** as Startup  
2. Ensure template assets are placed under:
3. Run the project → homepage opens

---



## 📌 6. API Endpoints (Sample)

### Users

### Appointments

### Doctors

---



## 📌 7. License
This project is submitted as part of the DEPI Graduation Project.

