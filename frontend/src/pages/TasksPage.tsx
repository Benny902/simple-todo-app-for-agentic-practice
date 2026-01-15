import { TaskList } from '../components/TaskList';
import { TaskInput } from '../components/TaskInput';
import { useTasks } from '../hooks/useTasks';

export const TasksPage = () => {
    const { 
        tasks, 
        loading, 
        error, 
        handleAddTask, 
        handleToggleTask 
    } = useTasks();

    return (
        <div className="tasks-page">
            <header className="page-header">
                <h1>Simple Task Manager</h1>
                <p>Simple task management for your day.</p>
            </header>
            
            <section className="input-section">
                <TaskInput onAddTask={handleAddTask} />
            </section>

            <section className="list-section">
                {error && <div className="error-message">{error}</div>}
                {loading ? (
                    <div className="loading-state">Loading tasks...</div>
                ) : (
                    <TaskList tasks={tasks} onToggleTask={handleToggleTask} />
                )}
            </section>
        </div>
    );
};
