import { ChangeDetectionStrategy, Component, computed, input } from '@angular/core';
import { NgIcon, provideIcons } from '@ng-icons/core';
import { lucideEllipsis } from '@ng-icons/lucide';
import { hlm } from '@spartan-ng/helm/utils';
import type { ClassValue } from 'clsx';

@Component({
	selector: 'hlm-breadcrumb-ellipsis',
	imports: [NgIcon],
	providers: [provideIcons({ lucideEllipsis })],
	changeDetection: ChangeDetectionStrategy.OnPush,
	template: `
		<span role="presentation" aria-hidden="true" [class]="_computedClass()">
			<ng-icon class="size-4" name="lucideEllipsis" />
			<span class="sr-only">{{ srOnlyText() }}</span>
		</span>
	`,
})
export class HlmBreadcrumbEllipsis {
	public readonly userClass = input<ClassValue>('', { alias: 'class' });
	/** Screen reader only text for the ellipsis */
	public readonly srOnlyText = input<string>('More');

	protected readonly _computedClass = computed(() => hlm('flex size-9 items-center justify-center', this.userClass()));
}
