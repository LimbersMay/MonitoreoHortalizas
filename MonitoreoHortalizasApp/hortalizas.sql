create table cama1
(
    idCama1 varchar(255) not null
        primary key,
    humedad int          not null,
    fecha   date         not null,
    hora    time         not null
);

create table cama2
(
    idCama2 varchar(255) not null
        primary key,
    humedad int          not null,
    fecha   date         not null,
    hora    time         not null
);

create table cama3
(
    idCama3 varchar(255) not null
        primary key,
    humedad int          not null,
    fecha   date         not null,
    hora    time         not null
);

create table cama4
(
    idCama4 varchar(255) charset utf8mb3 not null
        primary key,
    humedad int                          not null,
    fecha   date                         not null,
    hora    time                         not null
);

create table ciclosiembra
(
    cicloId     varchar(255)                       not null
        primary key,
    descripcion varchar(255) charset utf8mb3       null,
    fechaInicio datetime default CURRENT_TIMESTAMP not null,
    fechaFin    datetime default CURRENT_TIMESTAMP not null,
    ciclo       int                                not null,
    constraint ciclo_pk_2
        unique (cicloId)
);

create table cultivo
(
    cultivoId                 varchar(255)                                           not null
        primary key,
    nombreCultivo             varchar(100) charset utf8mb3 default ''                not null,
    germinacion               int                          default 0                 not null,
    fechaSiembra              datetime                     default CURRENT_TIMESTAMP not null,
    fechaCosecha              datetime                     default CURRENT_TIMESTAMP not null,
    tipoRiego                 varchar(50) charset utf8mb3  default ''                not null,
    gramaje                   double                       default 0                 not null,
    alturaMaxima              double                       default 0                 not null,
    alturaMinima              double                       default 0                 not null,
    temperaturaAmbienteMaxima int                          default 0                 not null,
    temperaturaAmbienteMinima int                          default 0                 not null,
    humedadAmbienteMaxima     int                          default 0                 not null,
    humedadAmbienteMinima     int                          default 0                 not null,
    humedadMinimaTierra       int                          default 0                 not null,
    presionBarometricaMaxima  int                          default 0                 not null,
    presionBarometricaMinima  int                          default 0                 not null,
    cicloId                   varchar(255) charset utf8mb3                           not null,
    humedadMaximaTierra       int                          default 0                 not null,
    descripcion               varchar(255) charset utf8mb3 default ''                not null
);

create index fk_cultivo_ciclo
    on cultivo (cicloId);

create table estado
(
    id            smallint auto_increment
        primary key,
    estado_nombre varchar(45) not null,
    estado_valor  smallint    not null
)
    engine = MyISAM
    collate = utf8mb3_unicode_ci;

create table genero
(
    id            smallint auto_increment,
    genero_nombre varchar(45) not null,
    constraint id
        unique (id)
)
    engine = MyISAM
    collate = utf8mb3_unicode_ci;

create table lineacultivo
(
    numeroLinea    int          not null,
    gramaje        double       null,
    lineaCultivoId varchar(255) not null
        primary key,
    cultivoId      varchar(255) not null,
    constraint lineaCultivo_cultivo_cultivoId_fk
        foreign key (cultivoId) references cultivo (cultivoId)
);

create table migration
(
    version    varchar(180) not null
        primary key,
    apply_time int          null
)
    engine = MyISAM
    charset = utf8mb3;

create table perfil
(
    id               int auto_increment
        primary key,
    user_id          int      not null,
    nombre           text     not null,
    apellido         text     not null,
    fecha_nacimiento datetime not null,
    genero_id        smallint not null,
    created_at       datetime not null,
    updated_at       datetime not null
)
    engine = MyISAM
    collate = utf8mb3_unicode_ci;

create index genero_id_2
    on perfil (genero_id);

create index user_id
    on perfil (user_id);

create table presionbarometrica
(
    idPresionBarometrica varchar(255)   not null
        primary key,
    presion              decimal(10, 3) null,
    temperatura          decimal(10, 3) null,
    altitud              decimal(10, 3) null,
    fecha                date           null,
    hora                 time           null
);

create table registrogerminacion
(
    registroGerminacionId  varchar(255)                                           not null
        primary key,
    temperaturaAmbiente    double                       default 0                 not null,
    humedadAmbiente        double                       default 0                 not null,
    numeroZurcosGerminados int                          default 0                 not null,
    broteAlturaMaxima      double                       default 0                 not null,
    broteAlturaMinima      double                       default 0                 not null,
    numeroMortandad        int                          default 0                 not null,
    observaciones          varchar(255) charset utf8mb3 default ''                not null,
    hojasAlturaMinima      double                       default 0                 not null,
    hojasAlturaMaxima      double                       default 0                 not null,
    linea                  int                          default 0                 not null,
    fechaRegistro          datetime                     default CURRENT_TIMESTAMP not null,
    cultivoId              varchar(255) charset utf8mb3                           not null
);

create index fk_registrogerminacion_cultivo
    on registrogerminacion (cultivoId);

create table riegomanual
(
    idRiegoManual  varchar(255)                not null
        primary key,
    fechaEncendido datetime                    null,
    fechaApagado   datetime                    null,
    volumen        decimal(10, 3)              null,
    cultivoId      varchar(50) charset utf8mb3 null
);

create index cultivoId
    on riegomanual (cultivoId);

create table rol
(
    id         smallint auto_increment
        primary key,
    rol_nombre varchar(45) not null,
    rol_valor  int         not null
)
    engine = MyISAM
    collate = utf8mb3_unicode_ci;

create table temperatura
(
    idTemperatura varchar(255)   not null
        primary key,
    temperatura   decimal(10, 3) not null,
    humedad       decimal(10, 3) not null,
    fecha         date           not null,
    hora          time           not null
);

create table tipo_usuario
(
    id                  smallint auto_increment
        primary key,
    tipo_usuario_nombre varchar(45) not null,
    tipo_usuario_valor  int         not null
)
    engine = MyISAM
    collate = utf8mb3_unicode_ci;

create table user
(
    id                   int auto_increment
        primary key,
    username             varchar(255)       not null,
    auth_key             varchar(32)        not null,
    password_hash        varchar(255)       not null,
    password_reset_token varchar(255)       null,
    email                varchar(255)       not null,
    rol_id               smallint default 1 not null,
    estado_id            smallint default 1 not null,
    tipo_usuario_id      smallint default 1 not null,
    created_at           datetime           not null,
    updated_at           datetime           not null,
    verification_token   varchar(255)       null,
    constraint email
        unique (email),
    constraint password_reset_token
        unique (password_reset_token),
    constraint username
        unique (username)
)
    collate = utf8mb3_unicode_ci;

create index id
    on user (id);

create table valvula
(
    idValvula      varchar(255)                not null
        primary key,
    fechaEncendido datetime                    null,
    fechaApagado   datetime                    null,
    volumen        decimal(10, 3)              null,
    cultivoId      varchar(50) charset utf8mb3 null
);

create index cultivoId
    on valvula (cultivoId);


