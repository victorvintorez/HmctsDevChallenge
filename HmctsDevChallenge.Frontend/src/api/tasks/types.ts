import {z} from 'zod'

export const CreateTaskSchema = z.object({
	title: z.string({error: 'Invalid Title'}).min(1, 'Title is required').max(255, 'Title cannot exceed 255 characters'),
	description: z.string({error: 'Invalid Description'}).max(2046, 'Description cannot exceed 2046 characters').optional(),
	status: z.string({error: 'Invalid Status'}).min(1, 'Status is required').max(63, 'Status cannot exceed 63 characters'),
	due: z.iso.datetime({
		local: true,
		error: 'Invalid Due Date'
	}).refine((date) => new Date(date) > new Date(), {message: 'Due date must be in the future'}),
})
export type CreateTaskType = z.infer<typeof CreateTaskSchema>;

export const ReadTaskSchema = z.object({
	id: z.int(),
	title: z.string(),
	description: z.string().optional(),
	status: z.string(),
	due: z.coerce.date(),
})
export type ReadTaskType = z.infer<typeof ReadTaskSchema>;