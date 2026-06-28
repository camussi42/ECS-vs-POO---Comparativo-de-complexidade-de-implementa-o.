// =============================================================
// SpawnerSystem.cs  —  Paradigma: ECS (Entity Component System)
// Sistema one-shot: instancia N entidades e se desativa automaticamente.
// Compatível com: Unity DOTS 1.x (Entities 1.0+)
// =============================================================
// CONTRASTE COM SpawnerOOP:
//   - SpawnerOOP acessa diretamente campos de BolinhaFisicaOOP (CBO alto)
//   - SpawnerSystem não conhece FisicaSystem nem DadosFisica de forma direta;
//     apenas configura os componentes via EntityCommandBuffer (CBO baixo)
//   - Após spawnar, a entidade spawner é destruída → sem polling desnecessário
// =============================================================

using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

/// <summary>
/// Sistema responsável por criar N entidades bolinha a partir do prefab ECS.
/// Executa uma única vez: destrói a entidade DadosSpawner após o spawn.
///
/// Ordem de execução: InitializationSystemGroup garante que as entidades
/// sejam criadas antes de FisicaSystem rodar no mesmo frame.
/// </summary>
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

    /// <summary>
    /// Roda apenas enquanto existir ao menos uma entidade DadosSpawner.
    /// Após destruir essa entidade, RequireForUpdate desativa o sistema.
    /// </summary>
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        // EntityCommandBuffer: registra operações estruturais (Instantiate, SetComponent,
        // DestroyEntity) para execução segura fora dos jobs Burst.
        // Allocator.Temp: liberado automaticamente ao final do frame.
        EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.Temp);

        // Semente determinística baseada no tempo decorrido
        uint semente = (uint)(SystemAPI.Time.ElapsedTime * 10000.0 + 1);
        Random rng = Random.CreateFromIndex(semente);

        // Itera sobre todas as entidades com DadosSpawner (tipicamente apenas 1)
        foreach (var (spawner, entidadeSpawner) in
                 SystemAPI.Query<RefRO<DadosSpawner>>().WithEntityAccess())
        {
            ref readonly DadosSpawner cfg = ref spawner.ValueRO;

            for (int i = 0; i < cfg.Quantidade; i++)
            {
                // Instancia uma cópia do prefab via ECB (thread-safe, Burst-friendly)
                Entity nova = ecb.Instantiate(cfg.Prefab);

                // Define posição inicial aleatória
                float3 posicao = new float3(
                    rng.NextFloat(-cfg.AreaSpawn, cfg.AreaSpawn),
                    rng.NextFloat(cfg.AlturaMin,  cfg.AlturaMax),
                    rng.NextFloat(-cfg.AreaSpawn, cfg.AreaSpawn)
                );
                ecb.SetComponent(nova, LocalTransform.FromPosition(posicao));

                // Define velocidade inicial: horizontal aleatória, vertical zero
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

            // Destrói a entidade spawner — esta é a última ação deste sistema
            ecb.DestroyEntity(entidadeSpawner);
        }

        // Aplica todos os comandos estruturais ao EntityManager
        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }
}
