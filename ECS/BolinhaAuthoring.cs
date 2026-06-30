using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;


/// Componente de edição para uma bolinha ECS.
public class BolinhaAuthoring : MonoBehaviour
{
    public float gravidade            = -9.8f;
    public float elasticidade         =  0.8f;
    public float limiteVelocidadeMin  =  0.1f;


    /// Entidade com LocalTransform + DadosFisica.
    private class Baker : Baker<BolinhaAuthoring>
    {
        public override void Bake(BolinhaAuthoring authoring)
        {
        
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


/// Componente de edição para o spawner ECS.
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
