namespace Engine.App;

public static class UnitMetadata
{
    public static class SoilType
    {
        /// <summary>
        /// Grass: Basic soil type, commonly found. Suitable for general plants and flowers.
        /// </summary>
        public const string Grass = nameof(Grass);

        /// <summary>
        /// Water: Not a soil type, but supports aquatic plants and ecosystems.
        /// </summary>
        public const string Water = nameof(Water);

        /// <summary>
        /// Fertile: Excellent for plants, provides plenty of flowers, speeds up growth, and increases yield.
        /// </summary>
        public const string Fertile = nameof(Fertile); // Плодородная почва

        /// <summary>
        /// Rocky: Hard to cultivate, plants grow slowly. Few flowers, but may contain rare resources (e.g., minerals).
        /// </summary>
        public const string Rocky = nameof(Rocky); // Каменная почва

        /// <summary>
        /// Sandy: Dries out quickly, plants require more care. Produces unique desert plants and flowers.
        /// </summary>
        public const string Sandy = nameof(Sandy); // Песчаная почва

        /// <summary>
        /// Clay: Retains moisture well but is heavy for plant roots. Suitable for certain plants that produce rare nectar.
        /// </summary>
        public const string Clay = nameof(Clay); // Глинистая почва

        /// <summary>
        /// Swamp: Moist and rich in vegetation, supports many flowers and attracts specific insect species.
        /// </summary>
        public const string Swamp = nameof(Swamp); // Болотистая почва

        /// <summary>
        /// Volcanic: Rich in minerals, supports unique exotic plants. Requires rare bees for pollination.
        /// </summary>
        public const string Volcanic = nameof(Volcanic); // Вулканическая почва

        /// <summary>
        /// Magical: Produces unusual glowing plants. Flowers yield special nectar for magical enhancements.
        /// </summary>
        public const string Magical = nameof(Magical); // Магическая почва

        /// <summary>
        /// Rainbow: Changes properties every few days. Produces random types of flowers.
        /// </summary>
        public const string Rainbow = nameof(Rainbow); // Радужная почва

        /// <summary>
        /// Polluted: Requires cleaning before use. After cleaning, can produce enhanced plants.
        /// </summary>
        public const string Polluted = nameof(Polluted); // Загрязнённая почва

        /// <summary>
        /// Technological: Artificial soil created by humans. Produces synthetic flowers for rare "tech-meta" nectar.
        /// </summary>
        public const string Technological = nameof(Technological); // Технологическая почва
    }
}