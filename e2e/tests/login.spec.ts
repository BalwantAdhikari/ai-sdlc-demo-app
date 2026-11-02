import { expect, test, type Page } from '@playwright/test';
import { LoginPage } from '../pages/login-page';

async function login(page: Page) {
  const response = await page.request.post('http://localhost:5000/api/auth/login', {
    data: { username: 'demo.approver', password: 'ChangeMe123!' },
  });

  expect(response.ok()).toBeTruthy();
  return await response.json();
}

test('demo approver can sign in', async ({ page }) => {
  const loginPage = new LoginPage(page);

  await loginPage.goto();
  await loginPage.signIn('demo.approver', 'ChangeMe123!');

  await expect(page.getByText('demo.approver (Approver)')).toBeVisible();
  await expect(page.getByRole('button', { name: 'Sign out' })).toBeVisible();
});

test('rejects an over-limit approval without changing invoice state', async ({ page }) => {
  const session = await login(page);
  const token = session.token as string;

  const createdInvoice = await page.request.post('http://localhost:5000/api/invoices', {
    headers: { Authorization: `Bearer ${token}` },
    data: { customerName: 'Large invoice', amount: 600000 },
  });

  expect(createdInvoice.ok()).toBeTruthy();
  const invoice = await createdInvoice.json();

  const submittedInvoice = await page.request.post(
    `http://localhost:5000/api/invoices/${invoice.id}/submit`,
    { headers: { Authorization: `Bearer ${token}` } }
  );

  expect(submittedInvoice.ok()).toBeTruthy();

  const approvalAttempt = await page.request.post(
    `http://localhost:5000/api/approvals/${invoice.id}/approve`,
    { headers: { Authorization: `Bearer ${token}` } }
  );

  expect(approvalAttempt.status()).toBe(409);
  const errorBody = await approvalAttempt.json();
  expect(errorBody.error).toContain('manager');

  const refreshedInvoice = await page.request.get(`http://localhost:5000/api/invoices/${invoice.id}`, {
    headers: { Authorization: `Bearer ${token}` },
  });

  expect(refreshedInvoice.ok()).toBeTruthy();
  const refreshedBody = await refreshedInvoice.json();
  expect(refreshedBody.status).toBe('PendingApproval');
});
