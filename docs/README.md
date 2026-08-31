# TimboLearn Documentation

Welcome to the TimboLearn technical documentation!

## What is TimboLearn?

TimboLearn is an enterprise-grade learning platform API built with .NET 10. It demonstrates modern backend architecture patterns including:

- ✅ Vertical Slice Architecture
- ✅ Hybrid authentication/authorization
- ✅ High-performance data access (EF Core + Dapper)
- ✅ AI integration patterns
- ✅ Developer-friendly local setup

## Quick Navigation

### 🚀 Getting Started
**New to TimboLearn? Start here!**

[→ Getting Started Guide](GettingStarted.md)

- 5-minute setup (no SQL Server required!)
- Auto-seeded demo data
- Swagger UI testing
- Test token generation

---

### 🏗️ Architecture
**Deep dive into technical design**

[→ Architecture Overview](Architecture.md)

- Key design decisions
- Domain model
- Authorization policies
- High-performance queries
- AI integration pattern

---

### 🧪 Testing
**Manual and automated testing strategies**

[→ Testing Guide](Testing.md)

- Swagger UI testing scenarios
- Test token usage
- Integration tests with Testcontainers
- curl examples
- Debugging tips

---

### 🔧 Troubleshooting
**Common issues and solutions**

[→ Troubleshooting Guide](Troubleshooting.md)

- Build errors
- Database issues
- Authentication problems
- Runtime errors
- Performance optimization

---

## Documentation Index

| Topic | Document | Description |
|---|---|---|
| Setup | [Getting Started](GettingStarted.md) | Clone, build, run, test |
| Architecture | [Architecture](Architecture.md) | Technical deep dive |
| Testing | [Testing](Testing.md) | Manual & automated tests |
| Support | [Troubleshooting](Troubleshooting.md) | Common issues |

---

## For GitHub Wiki Users

This `docs/` folder contains documentation ready to be copied to the GitHub Wiki:

1. **Enable Wiki** in repository Settings → Features
2. **Clone wiki repo:**
   ```bash
   git clone https://github.com/your-username/TimboLearn.wiki.git
   ```
3. **Copy these `.md` files** to the wiki repo
4. **Push to GitHub**

---

## Additional Resources

- **Main README**: [../README.md](../README.md) - Quick start and overview
- **Swagger UI**: http://localhost:5000/swagger (when running)
- **.NET Documentation**: https://docs.microsoft.com/dotnet/

---

**Need help?** Check the [Troubleshooting](Troubleshooting.md) guide or open an issue on GitHub.
