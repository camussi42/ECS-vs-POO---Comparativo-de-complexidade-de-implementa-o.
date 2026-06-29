# Benchmark Físico: OOP (MonoBehaviour) vs ECS (Unity DOTS)
## Estrutura dos arquivos

```
OOP/
  BolinhaOOP.cs       → MonoBehaviour da bolinha 
  SpawnerOOP.cs       → Gerenciador que instancia N bolinhas

ECS/
  ComponentesFisica.cs  → IComponentData 
  BolinhaAuthoring.cs   → conversão Inspector → entidades DOTS
  FisicaSystem.cs       → ISystem + IJobEntity 
  SpawnerSystem.cs      → Sistema instanciação
```

## Referências dos padrões utilizados

- Unity Technologies. *EntityComponentSystemSamples*. GitHub, 2024.
  https://github.com/Unity-Technologies/EntityComponentSystemSamples
- Unity Documentation. *ISystem vs SystemBase*. Entities 1.0, 2024.
- Antich, A. *Unity DOTS/ECS Performance: Amazing*. Medium / Superstring Theory, 2023.
- Redmond, P. et al. *Exploring the Theory and Practice of Concurrency in ECS*.
  OOPSLA 2025.
