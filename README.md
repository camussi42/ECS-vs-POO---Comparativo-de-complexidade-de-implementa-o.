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
## Passo a passo no Unity

### Packages necessários 
```
com.unity.entities          1.2.x ou superior
com.unity.burst             1.8.x ou superior
com.unity.collections       2.x
com.unity.mathematics       1.x
```

### Configuração da cena OOP
1. Crie um GameObject vazio → adicione `SpawnerOOP`
2. Crie um prefab com `BolinhaFisicaOOP` + `MeshRenderer` (bola)
3. Arraste o prefab para o campo `Prefab Bolinha` do SpawnerOOP
4. Configure `Quantidade` 

### Configuração da cena ECS
1. Ative o subscene: GameObject → New Sub Scene
2. Dentro da subscene, crie um GameObject → adicione `SpawnerAuthoring`
3. Crie um prefab com `BolinhaAuthoring` + qualquer MeshRenderer
4. Arraste o prefab para `Prefab Bolinha` do SpawnerAuthoring
5. Configure `Quantidade` igual ao cenário OOP

## Referências dos padrões utilizados

- Unity Technologies. *EntityComponentSystemSamples*. GitHub, 2024.
  https://github.com/Unity-Technologies/EntityComponentSystemSamples
- Unity Documentation. *ISystem vs SystemBase*. Entities 1.0, 2024.
- Antich, A. *Unity DOTS/ECS Performance: Amazing*. Medium / Superstring Theory, 2023.
- Redmond, P. et al. *Exploring the Theory and Practice of Concurrency in ECS*.
  OOPSLA 2025.
