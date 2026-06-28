# Benchmark Físico: OOP (MonoBehaviour) vs ECS (Unity DOTS)
## Guia de configuração e análise — Iniciação Científica

---

## Estrutura dos arquivos

```
OOP/
  BolinhaOOP.cs       → MonoBehaviour da bolinha (dados + lógica juntos)
  SpawnerOOP.cs       → Gerenciador que instancia N bolinhas

ECS/
  ComponentesFisica.cs  → IComponentData puras (dados puros, zero lógica)
  BolinhaAuthoring.cs   → Bakers: conversão Inspector → entidades DOTS
  FisicaSystem.cs       → ISystem + IJobEntity (lógica pura, zero dados)
  SpawnerSystem.cs      → Sistema one-shot de instanciação
```

---

## O que foi corrigido no código original

### OOP (original → corrigido)
| Problema original | Correção aplicada |
|---|---|
| `transform.position` acessado 3× por frame sem cache | `_transform` cacheado em `Awake()` |
| `new Vector3(...)` na colisão (alocação no hot-path) | Reutiliza struct local `posCorrigida` |
| Nenhum spawner — impossível testar em escala | `SpawnerOOP.cs` instancia N bolinhas |
| Sem threshold para micro-quiques | Constante `VelocidadeMinima` adicionada |

### ECS (original → corrigido)
| Problema original | Correção aplicada |
|---|---|
| **Sistema completamente ausente** (só o componente existia) | `FisicaSystem.cs` com `ISystem + IJobEntity` |
| Sem `[BurstCompile]` — sem compilação nativa | `[BurstCompile]` em sistema e job |
| Sem Baker/Authoring — impossível usar na cena | `BolinhaAuthoring.cs` com Bakers |
| Sem spawner | `SpawnerSystem.cs` com `EntityCommandBuffer` |
| `float3` mas sem `using Unity.Mathematics` | Imports corretos em todos os arquivos |
| `Translation` (API obsoleta pré-1.0) | `LocalTransform` (API 1.0+) |
| `SystemBase` (class, overhead de GC) | `ISystem` (struct, zero GC, suporte total a Burst) |

---

## Setup no Unity (passo a passo)

### Packages necessários (Package Manager)
```
com.unity.entities          1.2.x ou superior
com.unity.burst             1.8.x ou superior
com.unity.collections       2.x
com.unity.mathematics       1.x
```

### Configuração da cena OOP
1. Crie um GameObject vazio → adicione `SpawnerOOP`
2. Crie um prefab com `BolinhaFisicaOOP` + `MeshRenderer` (esfera)
3. Arraste o prefab para o campo `Prefab Bolinha` do SpawnerOOP
4. Configure `Quantidade` (ex: 1000, 5000, 10000)

### Configuração da cena ECS
1. Ative o subscene: GameObject → New Sub Scene
2. Dentro da subscene, crie um GameObject → adicione `SpawnerAuthoring`
3. Crie um prefab com `BolinhaAuthoring` + qualquer MeshRenderer
4. Arraste o prefab para `Prefab Bolinha` do SpawnerAuthoring
5. Configure `Quantidade` igual ao cenário OOP

> ⚠️ Os dois testes devem usar a mesma quantidade de objetos para comparação justa.

---

## O que o SonarQube vai capturar

### Métricas esperadas por paradigma

| Métrica | OOP (esperado) | ECS (esperado) | Justificativa |
|---|---|---|---|
| **DIT** (Depth of Inheritance Tree) | ≥ 1 (`MonoBehaviour`) | 0 (structs sem herança) | OOP herda de `MonoBehaviour`; ECS usa structs planas |
| **CBO** (Coupling Between Objects) | Alto (`SpawnerOOP` → `BolinhaFisicaOOP`) | Baixo (sistemas independentes) | OOP: Spawner acessa diretamente a classe Bolinha |
| **LCOM** (Lack of Cohesion) | Médio (dados + lógica na mesma classe) | Alto por design (intencionalmente separados) | Separação de concerns do ECS fragmenta coesão por classe individualmente |
| **CC** (Complexidade Ciclomática) | Concentrada em `Update()` | Distribuída entre `OnUpdate` + `Execute` | Mesma lógica, mas o ECS distribui em mais métodos |
| **WMC** (Weighted Methods per Class) | Baixo por classe (2–3 métodos) | Baixo por struct (1–2 métodos) | ECS tem mais arquivos, menos métodos por arquivo |

### Interpretação para a hipótese de pesquisa
- **DIT menor no ECS** → confirma redução de profundidade hierárquica
- **CBO menor no ECS** → confirma acoplamento mais fraco
- **CC comparável** → mesma complexidade lógica, diferente distribuição
- O SonarQube deve ser configurado com o perfil **Sonar Way C#**

---

## Parâmetros sugeridos para os experimentos

| Cenário | Quantidade de entidades | Objetivo |
|---|---|---|
| Pequeno | 100 | Verificar correção funcional |
| Médio | 1.000 | Baseline de métricas |
| Grande | 10.000 | Estresse de performance |
| Extra | 50.000 | Limite do OOP (esperado colapso de FPS) |

Use o **Unity Profiler** (Window → Analysis → Profiler) para medir:
- `Update.ScriptRunBehaviourUpdate` (custo total do OOP)
- `JobHandle.Complete` (custo total do ECS)

Use o **SonarQube Scanner for .NET** para análise estática:
```bash
dotnet sonarscanner begin /k:"ecs-oop-benchmark" /d:sonar.login="<token>"
dotnet build
dotnet sonarscanner end /d:sonar.login="<token>"
```

---

## Referências dos padrões utilizados

- Unity Technologies. *EntityComponentSystemSamples*. GitHub, 2024.
  https://github.com/Unity-Technologies/EntityComponentSystemSamples
- Unity Documentation. *ISystem vs SystemBase*. Entities 1.0, 2024.
- Antich, A. *Unity DOTS/ECS Performance: Amazing*. Medium / Superstring Theory, 2023.
- Redmond, P. et al. *Exploring the Theory and Practice of Concurrency in ECS*.
  OOPSLA 2025.
