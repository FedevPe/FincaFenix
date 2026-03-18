# 🌾 Sistema de Gestión de Órdenes de Trabajo — Fenix SA

> Aplicación web empresarial para la gestión de órdenes de trabajo agrícolas en múltiples unidades productivas.

![.NET](https://img.shields.io/badge/.NET_7-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
![Blazor](https://img.shields.io/badge/Blazor_Server-512BD4?style=for-the-badge&logo=blazor&logoColor=white)
![SQL Server](https://img.shields.io/badge/SQL_Server-CC2927?style=for-the-badge&logo=microsoft-sql-server&logoColor=white)
![Clean Architecture](https://img.shields.io/badge/Clean_Architecture-success?style=for-the-badge)

---

## 📋 Descripción

Sistema desarrollado para **Fenix SA**, empresa con múltiples unidades productivas agrícolas. Reemplazó un proceso **100% manual en papel** para el seguimiento y gestión de órdenes de trabajo de campo, brindando visibilidad en tiempo real a supervisores y gerencia.

### Problema que resolvía
Antes del sistema, los operarios de campo registraban sus actividades en papel al final de cada jornada. Esto generaba:
- ❌ Pérdida de información
- ❌ Procesamiento por lotes tardío (sin visibilidad en tiempo real)
- ❌ Imposibilidad de generar reportes de gestión confiables
- ❌ Acceso sin restricciones a la información operativa

---

## ✅ Funcionalidades

- **Gestión de órdenes de trabajo** — creación, asignación y seguimiento del ciclo de vida completo
- **Asignación a unidades productivas** — soporte para múltiples fincas/lotes
- **Seguimiento de operarios** — registro de actividades y horas trabajadas en tiempo real
- **Control de materiales** — registro de consumo de insumos por orden
- **Reportes de gestión** — consultas SQL complejas para toma de decisiones basada en datos
- **Control de acceso multi-rol** — Administrador · Supervisor · Operario
- **Formularios dinámicos en tiempo real** — carga de datos desde campo sin procesos por lotes

---

## 🏗️ Arquitectura

El sistema está construido sobre **Clean Architecture** con separación estricta de capas:

```
📁 Solution
├── 📂 UI (Blazor Server)          → Presentación e interacción con el usuario
├── 📂 Application                 → Casos de uso y lógica de negocio
├── 📂 Data Access                 → Repositorios, EF Core, SQL Server
└── 📂 Entities                    → Modelos de dominio y contratos
```

> Sin dependencias directas entre UI y base de datos, garantizando mantenibilidad a largo plazo.

---

## 🔐 Seguridad y Autenticación

Implementado con **ASP.NET Identity** y control de acceso basado en roles (RBAC):

| Rol | Permisos |
|---|---|
| **Administrador** | Acceso total al sistema, configuración, reportes |
| **Supervisor** | Gestión de órdenes, asignación de operarios, reportes |
| **Operario** | Carga de actividades y consumo de materiales |

---

## 🛠️ Stack Tecnológico

| Categoría | Tecnología |
|---|---|
| Backend | .NET 7 · C# · ASP.NET Core |
| Frontend | Blazor Server |
| ORM | Entity Framework Core |
| Base de datos | SQL Server |
| Autenticación | ASP.NET Identity |
| Arquitectura | Clean Architecture |
| Infraestructura | Windows Server |

---

## 📈 Impacto

- ✅ Eliminó el 100% del procesamiento manual en papel y la perdida de información
- ✅ Visibilidad en tiempo real de las operaciones de campo
- ✅ Reportes de gestión accionables para la gerencia
- ✅ Cero incidentes de acceso no autorizado desde el primer día en producción

---

## 👤 Autor

**Federico Pérez** — [LinkedIn](https://www.linkedin.com/in/fedevpe/) · [afedeperez2021@gmail.com](mailto:afedeperez2021@gmail.com)
