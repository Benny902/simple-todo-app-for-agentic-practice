const API_BASE_URL = 'http://localhost:5292';

import type { Task } from '../types';

export const api = {
    baseUrl: API_BASE_URL,

    getTasks: async (): Promise<Task[]> => {
        const response = await fetch(`${API_BASE_URL}/api/tasks`);
        if (!response.ok) {
            throw new Error('Failed to fetch tasks');
        }
        return response.json();
    },

    createTask: async (title: string): Promise<Task> => {
        const response = await fetch(`${API_BASE_URL}/api/tasks`, {
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
    }
};
