import { useState, useMemo } from 'react';
import { TaskList } from '../components/TaskList';
import { TaskInput } from '../components/TaskInput';
import { useTasks } from '../hooks/useTasks';
import type { Task } from '../types';

type FilterStatus = 'all' | 'active' | 'completed';
type SortOption = 'date-desc' | 'date-asc' | 'title-asc' | 'title-desc';

export const TasksPage = () => {
    const { 
        tasks, 
        loading, 
        error, 
        handleAddTask, 
        handleToggleTask,
        handleUpdateTask,
        handleDeleteTask
    } = useTasks();

    const [filter, setFilter] = useState<FilterStatus>('all');
    const [sortBy, setSortBy] = useState<SortOption>('date-desc');

    // Filter and sort tasks
    const filteredAndSortedTasks = useMemo(() => {
        let result = [...tasks];

        // Apply filter
        if (filter === 'active') {
            result = result.filter(task => !task.isCompleted);
        } else if (filter === 'completed') {
            result = result.filter(task => task.isCompleted);
        }

        // Apply sort
        result.sort((a, b) => {
            switch (sortBy) {
                case 'date-desc':
                    return new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime();
                case 'date-asc':
                    return new Date(a.createdAt).getTime() - new Date(b.createdAt).getTime();
                case 'title-asc':
                    return a.title.localeCompare(b.title);
                case 'title-desc':
                    return b.title.localeCompare(a.title);
                default:
                    return 0;
            }
        });

        return result;
    }, [tasks, filter, sortBy]);

    return (
        <div className="tasks-page">
            <header className="page-header">
                <h1>Simple Task Manager</h1>
                <p>Simple task management for your day.</p>
            </header>
            
            <section className="input-section">
                <TaskInput onAddTask={handleAddTask} />
            </section>

            <section className="controls-section">
                <div className="filter-controls">
                    <label htmlFor="filter">Filter:</label>
                    <select 
                        id="filter"
                        className="filter-select"
                        value={filter} 
                        onChange={(e) => setFilter(e.target.value as FilterStatus)}
                    >
                        <option value="all">All Tasks</option>
                        <option value="active">Active</option>
                        <option value="completed">Completed</option>
                    </select>
                </div>

                <div className="sort-controls">
                    <label htmlFor="sort">Sort by:</label>
                    <select 
                        id="sort"
                        className="sort-select"
                        value={sortBy} 
                        onChange={(e) => setSortBy(e.target.value as SortOption)}
                    >
                        <option value="date-desc">Date (Newest First)</option>
                        <option value="date-asc">Date (Oldest First)</option>
                        <option value="title-asc">Title (A-Z)</option>
                        <option value="title-desc">Title (Z-A)</option>
                    </select>
                </div>
            </section>

            <section className="list-section">
                {error && <div className="error-message">{error}</div>}
                {loading ? (
                    <div className="loading-state">Loading tasks...</div>
                ) : (
                    <TaskList 
                        tasks={filteredAndSortedTasks} 
                        onToggleTask={handleToggleTask}
                        onUpdateTask={handleUpdateTask}
                        onDeleteTask={handleDeleteTask}
                    />
                )}
            </section>
        </div>
    );
};
