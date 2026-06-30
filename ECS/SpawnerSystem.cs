using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;


/// Sistema responsável por criar N entidades bolinha a partir do prefab ECS.
/// Executa uma única vez e destrói a entidade DadosSpawner após o spawn.
///
/// Ordem de execução: InitializationSystemGroup garante que as entidades
/// sejam criadas antes de FisicaSystem rodar no mesmo frame.

[UpdateInGroup(typeof(InitializationSystemGroup))]
[BurstCompile]
public partial struct SpawnerSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        // Sistema fica inativo enquanto não houver nenhuma entidade com DadosSpawner
        state.RequireForUpdate<DadosSpawner>();
    }


    /// Roda apenas enquanto existir ao menos uma entidade DadosSpawner.

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        // EntityCommandBuffer: registra operações estruturais (Instantiate, SetComponent,
        // DestroyEntity)
        EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.Temp);

        // Seed baseada no tempo decorrido
        uint semente = (uint)(SystemAPI.Time.ElapsedTime * 10000.0 + 1);
        Random rng = Random.CreateFromIndex(semente);

        // Itera sobre todas as entidades com DadosSpawner
        foreach (var (spawner, entidadeSpawner) in
                 SystemAPI.Query<RefRO<DadosSpawner>>().WithEntityAccess())
        {
            ref readonly DadosSpawner cfg = ref spawner.ValueRO;

            for (int i = 0; i < cfg.Quantidade; i++)
            {
                // Instancia uma cópia do prefab via ECB
                Entity nova = ecb.Instantiate(cfg.Prefab);

                // Define posição inicial aleatória
                float3 posicao = new float3(
                    rng.NextFloat(-cfg.AreaSpawn, cfg.AreaSpawn),
                    rng.NextFloat(cfg.AlturaMin,  cfg.AlturaMax),
                    rng.NextFloat(-cfg.AreaSpawn, cfg.AreaSpawn)
                );
                ecb.SetComponent(nova, LocalTransform.FromPosition(posicao));

                // Define velocidade inicial
                ecb.SetComponent(nova, new DadosFisica
                {
                    Velocidade = new float3(
                        rng.NextFloat(-cfg.VelHorizMax, cfg.VelHorizMax),
                        0f,
                        rng.NextFloat(-cfg.VelHorizMax, cfg.VelHorizMax)
                    ),
                    Gravidade             = -9.8f,
                    Elasticidade          =  0.8f,
                    LimiteVelocidadeMinima =  0.1f
                });
            }

            // Destrói a entidade spawner
            ecb.DestroyEntity(entidadeSpawner);
        }

        // Aplica todos os comandos estruturais ao EntityManager
        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }
}
