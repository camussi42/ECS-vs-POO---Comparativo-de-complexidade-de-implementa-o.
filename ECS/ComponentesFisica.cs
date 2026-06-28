// =============================================================
// ComponentesFisica.cs  —  Paradigma: ECS (Entity Component System)
// Define os DADOS puros da simulação. Nenhuma linha de lógica aqui.
// Compatível com: Unity DOTS 1.x (Entities package 1.0+)
// =============================================================
// CONTRASTE COM OOP:
//   - Structs (não classes): sem herança, sem overhead de GC
//   - IComponentData garante layout de memória contíguo em chunks
//   - DIT = 0: nenhuma profundidade de herança (vs. OOP: DIT >= 1)
//   - CBO = 0: sem dependências entre componentes
// =============================================================

using Unity.Entities;
using Unity.Mathematics;

/// <summary>
/// Armazena o estado de física de cada bolinha.
/// Layout contíguo em memória → acesso cache-friendly pelo JobFisica.
/// </summary>
public struct DadosFisica : IComponentData
{
    public float3 Velocidade;
    public float  Gravidade;               // m/s²  (negativo para baixo)
    public float  Elasticidade;            // coeficiente de restituição [0..1]
    public float  LimiteVelocidadeMinima;  // threshold para zerar micro-quiques
}

/// <summary>
/// Configuração do spawner. Existirá como componente de uma única entidade
/// que será destruída logo após o spawn (one-shot pattern).
/// </summary>
public struct DadosSpawner : IComponentData
{
    public Entity Prefab;      // referência ao prefab ECS para instanciar
    public int    Quantidade;
    public float  AreaSpawn;
    public float  AlturaMin;
    public float  AlturaMax;
    public float  VelHorizMax;
}
