# 🏠 PropertyManagement API - Prueba Técnica

Sistema completo de gestión de propiedades inmobiliarias desarrollado en .NET 8 con arquitectura limpia, autenticación JWT, auditoría automática y documentación completa.

## 🚀 Características Principales

### ✅ **Implementaciones Completas**
- **CRUD Completo de Propiedades** con validaciones de negocio
- **Sistema de Autenticación JWT** con refresh tokens
- **Gestión de Archivos** (imágenes)
- **Auditoría Automática** de cambios con usuario y timestamp
- **Arquitectura Limpia** (Clean Architecture + CQRS + MediatR)
- **Validaciones** con FluentValidation
- **Testing Unitario** con NUnit + NSubstitute + FluentAssertions
- **Documentación Swagger** completa en español
- **Paginación y Filtros** avanzados
- **Manejo de Errores** centralizado con Result Pattern

### 🏗️ **Arquitectura Técnica**
- **.NET 8** con C# 12
- **Entity Framework Core** con SQL Server
- **Carter** para endpoints
- **MediatR** para patrón CQRS
- **JWT Bearer Authentication**
- **Serilog** para logging estructurado
- **Cache en Memoria** para datos maestros


## 🚀 Instalación y Ejecución

### **1. Clonar el Repositorio**


### **2. Configurar Base de Datos**

Actualizar connection string en appsettings.json

<img width="199" height="65" alt="image" src="https://github.com/user-attachments/assets/dcd386dc-9d41-457f-844c-24e0cad93524" />


### **3. Ejecutar la Aplicación**


### **Inicialización de Datos**
El sistema incluye **seeding automático** que se ejecuta al iniciar:
- **Usuario por defecto**: `danielcami782@gmail.com` / `P@ssw0rd!`
- **Países, estados y ciudades** de USA.
- **Tipos de identificación** 
- **Estados de propiedades** (Borrador, Listada, Vendida)

## 🔐 Autenticación y Autorización

### **Sistema de Autenticación**
- **JWT Bearer Tokens** para autenticación
- **Refresh Tokens** para renovación automática 
- **Access Tokens** con expiración 
- **Interceptor de auditoría** que captura usuario actual


### **Flujo de Autenticación**

#### **1. Login Inicial**

#### **2. Usar Access Token**

#### **3. Renovar Token (cuando expire)**


### **Endpoints Protegidos**
Todos los endpoints excepto `/auth/*` requieren autenticación:
- `/api/properties/*` - Gestión de propiedades
- `/api/files/*` - Gestión de archivos
- `/api/countries/*` - Datos geográficos
- `/api/master/*` - Datos maestros




