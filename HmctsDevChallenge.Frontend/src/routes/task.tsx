import {createFileRoute, Link} from '@tanstack/react-router'
import {ReadTaskSchema} from "../api/tasks/types.ts";
import * as GovUK from "govuk-react";

const TaskRouteComponent = () => {
	const task = Route.useSearch()

	return (
		<>
			<GovUK.BackLink as={Link} to="/">Back</GovUK.BackLink>
			<GovUK.Heading size='L'>
				<GovUK.Caption size='M'>Task {task.id}</GovUK.Caption>
				{task.title}
			</GovUK.Heading>
			{task.description && (
				<>
					<GovUK.Heading size='M'>Description</GovUK.Heading>
					<GovUK.InsetText>{task.description}</GovUK.InsetText>
				</>
			)}
			<GovUK.Paragraph>
				{`**Status:** ${task.status}`}
			</GovUK.Paragraph>
			<GovUK.Paragraph>
				{`**Due Date:** ${new Date(task.due).toLocaleString()}`}
			</GovUK.Paragraph>
		</>
	)
}

export const Route = createFileRoute('/task')({
	component: TaskRouteComponent,
	validateSearch: ReadTaskSchema
})
