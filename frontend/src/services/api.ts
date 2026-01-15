import { config } from '../config';
import type { Task } from '../types';

export const api = {
    baseUrl: config.API_URL,

    getTasks: async (): Promise<Task[]> => {
        const response = await fetch(`${config.API_URL}/api/tasks`);
        if (!response.ok) {
            throw new Error('Failed to fetch tasks');
        }
        return response.json();
    },

    createTask: async (title: string): Promise<Task> => {
        const response = await fetch(`${config.API_URL}/api/tasks`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify({ title }),
        });
        if (!response.ok) {
            throw new Error('Failed to create task');
        }
        return response.json();
    },

    updateTask: async (id: string, updates: Partial<Task>): Promise<Task> => {
        const response = await fetch(`${config.API_URL}/api/tasks/${id}`, {
            method: 'PATCH',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify(updates),
        });
        if (!response.ok) {
            throw new Error('Failed to update task');
        }
        return response.json();
    }
};
