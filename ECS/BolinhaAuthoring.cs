// =============================================================
// BolinhaAuthoring.cs  —  Paradigma: ECS (Entity Component System)
// Ponte entre o Inspector do Unity e o mundo de entidades DOTS.
// O processo de "Baking" converte MonoBehaviours de edição em
// componentes de dados puros — sem custo em tempo de execução.
// Compatível com: Unity DOTS 1.x (Entities 1.0+, Baking API)
// =============================================================

using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

// ──────────────────────────────────────────────────────────────
// AUTHORING 1: Configuração de uma bolinha individual
// ──────────────────────────────────────────────────────────────

/// <summary>
/// Componente de edição para uma bolinha ECS.
/// Visível no Inspector; convertido em DadosFisica durante o Baking.
/// </summary>
public class BolinhaAuthoring : MonoBehaviour
{
    public float gravidade            = -9.8f;
    public float elasticidade         =  0.8f;
    public float limiteVelocidadeMin  =  0.1f;

    /// <summary>
    /// Baker: executa em tempo de edição/build para converter os dados
    /// do MonoBehaviour em componentes ECS puros.
    /// Resultado: entidade com LocalTransform + DadosFisica.
    /// </summary>
    private class Baker : Baker<BolinhaAuthoring>
    {
        public override void Bake(BolinhaAuthoring authoring)
        {
            // TransformUsageFlags.Dynamic: entidade terá LocalTransform mutável
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);

            AddComponent(entity, new DadosFisica
            {
                Velocidade            = float3.zero,
                Gravidade             = authoring.gravidade,
                Elasticidade          = authoring.elasticidade,
                LimiteVelocidadeMinima = authoring.limiteVelocidadeMin
            });
        }
    }
}

// ──────────────────────────────────────────────────────────────
// AUTHORING 2: Configuração do Spawner
// ──────────────────────────────────────────────────────────────

/// <summary>
/// Componente de edição para o spawner ECS.
/// Convertido em DadosSpawner durante o Baking.
/// </summary>
public class SpawnerAuthoring : MonoBehaviour
{
    public GameObject prefabBolinha;
    public int        quantidade  = 1000;
    public float      areaSpawn   = 10f;
    public float      alturaMin   = 5f;
    public float      alturaMax   = 15f;
    public float      velHorizMax = 3f;

    private class Baker : Baker<SpawnerAuthoring>
    {
        public override void Bake(SpawnerAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.None);

            AddComponent(entity, new DadosSpawner
            {
                // Converte o GameObject prefab em uma referência de entidade ECS
                Prefab      = GetEntity(authoring.prefabBolinha, TransformUsageFlags.Dynamic),
                Quantidade  = authoring.quantidade,
                AreaSpawn   = authoring.areaSpawn,
                AlturaMin   = authoring.alturaMin,
                AlturaMax   = authoring.alturaMax,
                VelHorizMax = authoring.velHorizMax
            });
        }
    }
}
