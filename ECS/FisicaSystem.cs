// =============================================================
// FisicaSystem.cs  —  Paradigma: ECS (Entity Component System)
// SISTEMA de física: contém apenas LÓGICA, opera sobre dados externos.
// Compatível com: Unity DOTS 1.x (Entities 1.0+)
// =============================================================
// CONTRASTE ARQUITETURAL COM OOP:
//   Sistema não armazena estado próprio nem conhece entidades específicas.
//   Opera sobre QUALQUER entidade que possua {LocalTransform, DadosFisica}.
//   CBO: zero acoplamento com outras classes — busca dados via query implícita.
//   A lógica é IDÊNTICA ao Update() do OOP; a ARQUITETURA é radicalmente diferente.
// =============================================================

using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

/// <summary>
/// Sistema que agenda o job de física para todas as entidades com
/// componentes {LocalTransform + DadosFisica}.
///
/// ISystem (struct) vs SystemBase (class):
///   - Sem alocação de heap; sem overhead de GC
///   - Suporte completo ao Burst Compiler
///   - Requer [BurstCompile] tanto no struct quanto em cada método
/// </summary>
[BurstCompile]
public partial struct FisicaSystem : ISystem
{
    /// <summary>
    /// Chamado uma vez na criação do sistema.
    /// RequireForUpdate garante que OnUpdate só rode quando houver
    /// ao menos uma entidade com DadosFisica — evita overhead em cenas vazias.
    /// </summary>
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<DadosFisica>();
    }

    /// <summary>
    /// Chamado toda frame. Cria e agenda o JobFisica em paralelo.
    /// ScheduleParallel(): distribui entidades entre threads do Job System —
    /// comportamento impossível com MonoBehaviour.Update() no OOP.
    /// </summary>
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

/// <summary>
/// Job de física: compilado para código nativo otimizado via Burst.
/// Execute() é chamado uma vez por entidade que satisfaça a query implícita
/// (entidades com LocalTransform + DadosFisica), em múltiplas threads.
///
/// Mesma lógica do Update() do OOP — comparação justa de COMPLEXIDADE,
/// não de lógica.
/// </summary>
[BurstCompile]
public partial struct JobFisica : IJobEntity
{
    // DeltaTime capturado no OnUpdate (thread-safe; somente leitura no job)
    public float DeltaTime;

    /// <summary>
    /// Opera sobre UMA entidade por chamada.
    /// ref  = acesso de leitura e escrita (modifica o componente)
    /// Sem ramificações desnecessárias; layout de memória linear → SIMD-friendly.
    /// </summary>
    public void Execute(ref LocalTransform transform, ref DadosFisica fisica)
    {
        // 1. Integração de Euler semi-implícita: atualiza velocidade
        fisica.Velocidade.y += fisica.Gravidade * DeltaTime;

        // 2. Atualiza posição
        transform.Position += fisica.Velocidade * DeltaTime;

        // 3. Colisão com o plano y = 0
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
