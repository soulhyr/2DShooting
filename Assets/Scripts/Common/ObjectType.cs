public enum ObjectType
{
    None = 99999,
    /// <summary>
    /// 파란색 비행기
    /// </summary>
    Player1 = 0,
    /// <summary>
    /// 초록색 비행기
    /// </summary>
    Player2 = 1,
    /// <summary>
    /// 붉은색 비행기
    /// </summary>
    Player3 = 2,
    /// <summary>
    /// 작은 적
    /// </summary>
    EnemyA = 100,
    /// <summary>
    /// 세모 적
    /// </summary>
    EnemyB = 101,
    /// <summary>
    /// 커다란 적
    /// </summary>
    EnemyC = 102,
    /// <summary>
    /// Boss A
    /// </summary>
    BossA = 150,
    BulletEnemyA = 201,
    BulletEnemyB = 202,
    BulletEnemyC = 203,
    BulletEnemyD = 204,
    BulletPlayerA = 250,
    BulletPlayerB = 251,
    BulletFollow = 252,
    ItemBoom = 300,
    ItemCoin = 301,
    ItemPower = 302,
    /// <summary>
    /// 보조 비행기 1
    /// </summary>
    Follow1 = 400,
    /// <summary>
    /// 폭탄 터지는 효과
    /// </summary>
    FxBoom = 500,
    /// <summary>
    /// 물체 폭발하는 효과
    /// </summary>
    FxExplosion = 501,
    PlayerSlot = 999
}