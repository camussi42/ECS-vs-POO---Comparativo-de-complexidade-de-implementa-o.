//   - Structs (não classes) são sem herança, e sem overhead ;

using Unity.Entities;
using Unity.Mathematics;

/// Armazena o estado de física de cada bolinha.
public struct DadosFisica : IComponentData
{
    public float3 Velocidade;
    public float  Gravidade;               // m/s²  (negativo para baixo)
    public float  Elasticidade;            // coeficiente de restituição [0..1]
    public float  LimiteVelocidadeMinima;  // threshold 
}


/// Configuração do spawner. 
public struct DadosSpawner : IComponentData
{
    public Entity Prefab;      
    public int    Quantidade;
    public float  AreaSpawn;
    public float  AlturaMin;
    public float  AlturaMax;
    public float  VelHorizMax;
}
