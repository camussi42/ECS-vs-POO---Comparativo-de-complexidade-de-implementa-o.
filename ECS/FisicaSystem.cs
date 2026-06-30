using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;


/// Sistema que agenda o job de física para todas as entidades com
/// componentes {LocalTransform + DadosFisica}.
[BurstCompile]
public partial struct FisicaSystem : ISystem
{

    /// RequireForUpdate garante que OnUpdate só rode quando houver
    /// ao menos uma entidade com DadosFisica.
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<DadosFisica>();
    }


    /// Chamado toda frame
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        new JobFisica
        {
            DeltaTime = SystemAPI.Time.DeltaTime
        }.ScheduleParallel();
        // A dependência de job é propagada automaticamente pelo sistema de scheduling
    }
}


/// Mesma lógica do Update() do OOP 
[BurstCompile]
public partial struct JobFisica : IJobEntity
{
    public float DeltaTime;


    /// Opera sobre UMA entidade por chamada.
    /// ref  = acesso de leitura e escrita (modifica o componente)
    public void Execute(ref LocalTransform transform, ref DadosFisica fisica)
    {
        // 1. Integração de Euler 
        fisica.Velocidade.y += fisica.Gravidade * DeltaTime;

        // 2. Atualiza posição
        transform.Position += fisica.Velocidade * DeltaTime;

        // 3. Colisão com o plano y 
        if (transform.Position.y < 0f)
        {
            // Correção de penetração
            transform.Position = new float3(
                transform.Position.x,
                0f,
                transform.Position.z
            );

            // Resposta de colisão: lei de restituição de Newton
            fisica.Velocidade.y = -fisica.Velocidade.y * fisica.Elasticidade;

            // Damping: zera velocidade insignificante
            if (math.abs(fisica.Velocidade.y) < fisica.LimiteVelocidadeMinima)
            {
                fisica.Velocidade.y = 0f;
            }
        }
    }
}
