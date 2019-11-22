# ScanPelleng
Программа для проведения электродинамических расчётов поля апертуры антенны с учётом проволочных объектов в ближней зоне антенны. 
Нахождение поверхностных токов производится методом поверхностных интегральных уравнений. 
Графический интерфейс и логика программы написаны на С# в Windows Forms c использованием SlimDX 
для трехмерного отображения математической модели и компоненты построителя графиков ZedGraph.
Решатель вынесен в подключемые библиотеки на С++ с применением технологии OpenMP.
