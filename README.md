# khirblog — ASP.NET Core MVC Blog Application

A full-featured blog platform built with ASP.NET Core MVC, Entity Framework Core, and ASP.NET Identity. Features a clean, modern UI powered by Tailwind CSS.

---

## 🚀 Features

- **Authentication** — Register, Login, Logout with ASP.NET Identity
- **Blog Posts** — Create, Read, Update, Delete with image uploads
- **Categories** — Filter posts by category with dropdown navigation
- **Tags** — Tag posts for better organization
- **Comments** — Add, Edit, Delete comments on posts
- **Likes** — Like/Unlike posts
- **User Profiles** — View and edit profile, change password
- **Admin Panel** — Manage posts, categories, tags, users, and comments
- **Responsive UI** — Mobile-first design with Tailwind CSS
- **Slug-based URLs** — SEO-friendly post URLs

---

## 🛠️ Tech Stack

| Layer | Technology |
|---|---|
| Framework | ASP.NET Core MVC (.NET 8) |
| ORM | Entity Framework Core |
| Database | SQL Server |
| Authentication | ASP.NET Core Identity |
| UI Styling | Tailwind CSS (CDN) |
| Icons | Font Awesome 6 |
| Architecture | Repository Pattern |

---

## 🏗️ Project Architecture

```
BlogApp/
├── Controllers/                    # MVC Controllers
│   ├── BaseController.cs           # Base controller (loads categories for header)
│   ├── HomeController.cs           # Home page
│   ├── BlogPostController.cs       # Blog post CRUD
│   ├── AccountController.cs        # Auth (Login, Register, Logout)
│   ├── CategoryController.cs       # Category listing & filtering
│   ├── CommentController.cs        # Comment Add, Edit, Delete
│   ├── LikeController.cs           # Like toggle
│   ├── ProfileController.cs        # User profile management
│   ├── PageController.cs           # Static pages (About, Contact, Privacy, Terms)
│   └── Admin/
│       ├── AdminDashboardController.cs
│       ├── AdminBlogPostController.cs
│       ├── AdminCategoryController.cs
│       ├── AdminTagController.cs
│       ├── AdminUserController.cs
│       └── AdminCommentController.cs
│
├── Models/                         # Domain Models
│   ├── ApplicationUser.cs          # Extended Identity User
│   ├── BlogPost.cs
│   ├── Category.cs
│   ├── Tag.cs
│   ├── BlogPostTag.cs              # Many-to-many join table
│   ├── Comment.cs
│   └── Like.cs
│
├── ViewModels/                     # View Models (DTOs for views)
│   ├── RegisterViewModel.cs
│   ├── LoginViewModel.cs
│   ├── BlogPostViewModel.cs
│   ├── BlogPostFormViewModel.cs
│   ├── BlogPostIndexViewModel.cs
│   ├── BlogPostDetailViewModel.cs
│   ├── HomeViewModel.cs
│   ├── CategoryViewModel.cs
│   ├── CategoryDetailViewModel.cs
│   ├── TagViewModel.cs
│   ├── CommentViewModel.cs
│   ├── ProfileViewModel.cs
│   └── Admin/
│       ├── AdminDashboardViewModel.cs
│       ├── AdminBlogPostViewModel.cs
│       ├── AdminCategoryViewModel.cs
│       ├── AdminTagViewModel.cs
│       ├── AdminUserViewModel.cs
│       └── AdminCommentViewModel.cs
│
├── Repositories/                   # Repository Pattern
│   ├── Interfaces/
│   │   ├── IBlogPostRepository.cs
│   │   ├── ICategoryRepository.cs
│   │   ├── ITagRepository.cs
│   │   ├── ICommentRepository.cs
│   │   ├── ILikeRepository.cs
│   │   └── IUserRepository.cs
│   └── Implementations/
│       ├── BlogPostRepository.cs
│       ├── CategoryRepository.cs
│       ├── TagRepository.cs
│       ├── CommentRepository.cs
│       ├── LikeRepository.cs
│       └── UserRepository.cs
│
├── Services/                       # Business Logic Services
│   ├── Interfaces/
│   │   ├── IAuthService.cs
│   │   └── IImageService.cs
│   └── Implementations/
│       ├── AuthService.cs
│       └── ImageService.cs
│
├── Data/                           # Database Layer
│   ├── ApplicationDbContext.cs     # EF Core DbContext
│   └── SeedData.cs                 # Seeds roles, admin user, categories, tags
│
├── Views/                          # Razor Views
│   ├── Shared/
│   │   ├── _Layout.cshtml          # Main public layout
│   │   ├── _AdminLayout.cshtml     # Admin panel layout
│   │   ├── _AuthLayout.cshtml      # Auth pages layout (login/register)
│   │   ├── _Header.cshtml          # Header partial
│   │   ├── _Footer.cshtml          # Footer partial
│   │   ├── _ViewImports.cshtml
│   │   └── Error.cshtml
│   ├── Home/
│   │   └── Index.cshtml
│   ├── BlogPost/
│   │   ├── Index.cshtml
│   │   ├── Detail.cshtml
│   │   ├── Create.cshtml
│   │   ├── Edit.cshtml
│   │   └── MyPosts.cshtml
│   ├── Account/
│   │   ├── Login.cshtml
│   │   └── Register.cshtml
│   ├── Category/
│   │   ├── Index.cshtml            # Posts by category
│   │   └── All.cshtml              # All categories
│   ├── Profile/
│   │   ├── Index.cshtml
│   │   ├── Edit.cshtml
│   │   └── ChangePassword.cshtml
│   ├── Page/
│   │   ├── About.cshtml
│   │   ├── Contact.cshtml
│   │   ├── Privacy.cshtml
│   │   └── Terms.cshtml
│   └── Admin/
│       ├── AdminDashboard/
│       │   └── Index.cshtml
│       ├── AdminBlogPost/
│       │   └── Index.cshtml
│       ├── AdminCategory/
│       │   ├── Index.cshtml
│       │   ├── Create.cshtml
│       │   └── Edit.cshtml
│       ├── AdminTag/
│       │   ├── Index.cshtml
│       │   ├── Create.cshtml
│       │   └── Edit.cshtml
│       ├── AdminUser/
│       │   └── Index.cshtml
│       └── AdminComment/
│           └── Index.cshtml
│
├── wwwroot/
│   └── uploads/
│       └── images/                 # Uploaded post images
│
└── Program.cs                      # App entry point & DI registration
```

---

## 🗄️ Database Schema

```
ApplicationUser (Identity)
    ├── Id, Email, UserName
    ├── FirstName, LastName
    └── IsAdmin

BlogPost
    ├── Id, Title, Slug
    ├── Content, Excerpt
    ├── ImageUrl, IsPublished
    ├── CreatedAt, UpdatedAt
    ├── AuthorId → ApplicationUser
    └── CategoryId → Category

Category
    ├── Id, Name, Description
    └── BlogPosts (navigation)

Tag
    ├── Id, Name, Description
    └── BlogPostTags (navigation)

BlogPostTag (join table)
    ├── BlogPostId → BlogPost
    └── TagId → Tag

Comment
    ├── Id, Content, CreatedAt
    ├── BlogPostId → BlogPost
    └── UserId → ApplicationUser

Like
    ├── Id
    ├── BlogPostId → BlogPost
    └── UserId → ApplicationUser (unique per user per post)
```

---

## 🔐 Authorization

| Role | Permissions |
|---|---|
| **Guest** | View posts, categories, tags |
| **User** | + Create posts, comment, like, manage own posts |
| **Admin** | + Full access to admin panel, manage all content and users |

---

## 🧱 Design Patterns

### Repository Pattern
Abstracts data access logic from business logic. Each entity has an interface and implementation:
```
IBlogPostRepository → BlogPostRepository
ICategoryRepository → CategoryRepository
...
```

### Service Layer
Business logic lives in services, not controllers:
```
IAuthService  → AuthService  (register, login, logout, change password)
IImageService → ImageService (upload, delete images)
```

### Base Controller
`BaseController` loads categories for the header dropdown on every page automatically via `OnActionExecutionAsync`.

---

## ⚙️ Getting Started

### Prerequisites
- .NET 8 SDK
- SQL Server
- Visual Studio 2022 or VS Code

### Setup

**1. Clone the repository:**
```bash
git clone https://github.com/yourusername/BlogApp.git
cd BlogApp
```

**2. Update connection string in `appsettings.json`:**
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=BlogAppDb;Trusted_Connection=True;"
  }
}
```

**3. Apply migrations:**
```bash
dotnet ef database update
```

**4. Run the application:**
```bash
dotnet run
```

**5. Default admin credentials:**
```
Email:    admin@blogapp.com
Password: Admin@123
```

---

## 🌐 URL Structure

| URL | Description |
|---|---|
| `/` | Home page |
| `/BlogPost` | All published posts |
| `/BlogPost/Detail/{slug}` | Single post |
| `/BlogPost/Create` | Create post (auth required) |
| `/BlogPost/MyPosts` | Current user's posts |
| `/Category/All` | All categories |
| `/Category/Index/{id}` | Posts by category |
| `/BlogPost?categoryId={id}` | Filter posts by category |
| `/Profile` | User profile |
| `/Profile/Edit` | Edit profile |
| `/Profile/ChangePassword` | Change password |
| `/Account/Login` | Login |
| `/Account/Register` | Register |
| `/Page/About` | About page |
| `/Page/Contact` | Contact page |
| `/Page/Privacy` | Privacy policy |
| `/Page/Terms` | Terms of service |
| `/Admin/Dashboard/Index` | Admin dashboard |
| `/Admin/AdminBlogPost` | Manage all posts |
| `/Admin/AdminCategory` | Manage categories |
| `/Admin/AdminTag` | Manage tags |
| `/Admin/AdminUser` | Manage users |
| `/Admin/AdminComment` | Moderate comments |

---

## 📦 Dependency Injection Registration

```csharp
// Repositories
builder.Services.AddScoped<IBlogPostRepository, BlogPostRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ITagRepository, TagRepository>();
builder.Services.AddScoped<ICommentRepository, CommentRepository>();
builder.Services.AddScoped<ILikeRepository, LikeRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

// Services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IImageService, ImageService>();
```

---

## 📁 Image Uploads

Post cover images are stored locally at:
```
wwwroot/uploads/images/
```
- Max file size: **5MB**
- Supported formats: **PNG, JPG, GIF, WEBP**
- Filenames are auto-generated using `Guid` to avoid conflicts

---

## 🎨 UI Components

| Component | Description |
|---|---|
| `_Layout.cshtml` | Main layout with sticky header and footer |
| `_AdminLayout.cshtml` | Admin layout with top bar and sidebar |
| `_AuthLayout.cshtml` | Minimal layout for login/register pages |
| `_Header.cshtml` | Sticky nav with category dropdown and user menu |
| `_Footer.cshtml` | Footer with links and newsletter signup |

---

## 🧪 Seeded Data

On first run the application automatically seeds:

- **Roles** — `Admin`, `User`
- **Admin User** — `admin@blogapp.com / Admin@123`
- **Categories** — Technology, Design, Lifestyle, Coding, Creativity
- **Tags** — Technology, Programming, Lifestyle, Travel, Food

---

*Built with ❤️ using ASP.NET Core MVC*
